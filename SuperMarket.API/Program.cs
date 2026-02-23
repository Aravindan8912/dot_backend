using System.Linq;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SuperMarket.Infrastructure;
using SuperMarket.Infrastructure.Persistence;
using SuperMarket.Application.UseCases.Categories;
using SuperMarket.Application.UseCases.Customers;
using SuperMarket.Application.UseCases.Products;
using SuperMarket.Application.UseCases.Orders;
using SuperMarket.Application.UseCases.Auth;
using SuperMarket.Application.UseCases.Address;
using SuperMarket.Application.UseCases.Payments;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.WriteIndented = false;
    });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();

// CORS configuration for frontend
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularApp", policy =>
    {
        policy.WithOrigins("http://localhost:4200", "http://localhost:5173", "http://localhost:3001")
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

builder.Services.AddInfrastructure(builder.Configuration);

// Auth use cases
builder.Services.AddScoped<LoginUseCase>();
builder.Services.AddScoped<RefreshTokenUseCase>();
builder.Services.AddScoped<RegisterUserUseCase>();

// Business use cases
builder.Services.AddScoped<CreateCategoryUseCase>();
builder.Services.AddScoped<CreateCustomerUseCase>();
builder.Services.AddScoped<CreateProductUseCase>();
builder.Services.AddScoped<CreateOrderUseCase>();
builder.Services.AddScoped<UpdateProductUseCase>();
builder.Services.AddScoped<CreateAddressUseCase>();
builder.Services.AddScoped<GetAddressesByCustomerIdUseCase>();
builder.Services.AddScoped<CreatePaymentUseCase>();
builder.Services.AddScoped<GetPaymentByOrderIdUseCase>();

// JWT authentication (production-grade: issuer, audience, lifetime validation)
var jwtSecret = builder.Configuration["Jwt:SecretKey"] ?? throw new InvalidOperationException("Jwt:SecretKey is required.");
var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? "SuperMarket.Api";
var jwtAudience = builder.Configuration["Jwt:Audience"] ?? "SuperMarket.Client";

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret)),
        ValidateIssuer = true,
        ValidIssuer = jwtIssuer,
        ValidateAudience = true,
        ValidAudience = jwtAudience,
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };
    // Log why JWT failed (helps debug 401)
    options.Events = new Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerEvents
    {
        OnAuthenticationFailed = ctx =>
        {
            ctx.Response.StatusCode = 401;
            var reason = ctx.Exception is SecurityTokenExpiredException
                ? "Token has expired. Use POST /api/auth/refresh to get a new access token."
                : (ctx.Exception?.Message ?? "Invalid token.");
            ctx.Response.ContentType = "application/json";
            return ctx.Response.WriteAsJsonAsync(new { error = "Unauthorized", message = reason });
        },
        OnChallenge = ctx =>
        {
            if (ctx.AuthenticateFailure != null)
                return Task.CompletedTask; // OnAuthenticationFailed already wrote the response
            ctx.Response.StatusCode = 401;
            ctx.Response.ContentType = "application/json";
            var message = string.IsNullOrEmpty(ctx.Request.Headers.Authorization) || !ctx.Request.Headers.Authorization.ToString().StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase)
                ? "Missing or invalid Authorization header. Use: Authorization: Bearer <accessToken>"
                : "Unauthorized";
            return ctx.Response.WriteAsJsonAsync(new { error = "Unauthorized", message });
        }
    };
});
builder.Services.AddAuthorization();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    // Retry migrations until MySQL is reachable (e.g. when started via Docker Compose).
    const int maxRetries = 10;
    const int delayMs = 3000;
    for (var i = 0; i < maxRetries; i++)
    {
        try
        {
            db.Database.Migrate();
            break;
        }
        catch (Exception ex) when (i < maxRetries - 1)
        {
            if (ex.Message.Contains("Unable to connect", StringComparison.OrdinalIgnoreCase))
                await Task.Delay(delayMs);
            else
                throw;
        }
    }
    if (app.Environment.IsDevelopment())
        await scope.ServiceProvider.SeedDefaultAdminIfNeededAsync();
}

// Handle OPTIONS (preflight) first: return 200 + JSON body so no 204 empty response (avoids "Http failure during parsing")
var allowedOrigins = new[] { "http://localhost:4200", "http://localhost:5173", "http://localhost:3001" };
app.Use(async (ctx, next) =>
{
    if (ctx.Request.Method == "OPTIONS")
    {
        var origin = (string?)ctx.Request.Headers["Origin"];
        if (!string.IsNullOrEmpty(origin) && allowedOrigins.Contains(origin))
        {
            ctx.Response.Headers.Append("Access-Control-Allow-Origin", origin);
            ctx.Response.Headers.Append("Access-Control-Allow-Credentials", "true");
        }
        ctx.Response.Headers.Append("Access-Control-Allow-Methods", "GET, POST, PUT, PATCH, DELETE, OPTIONS");
        ctx.Response.Headers.Append("Access-Control-Allow-Headers", "Authorization, Content-Type, Accept");
        ctx.Response.Headers.Append("Access-Control-Max-Age", "86400");
        ctx.Response.StatusCode = StatusCodes.Status200OK;
        ctx.Response.ContentType = "application/json";
        await ctx.Response.WriteAsync("{}");
        return;
    }
    await next();
});

// CORS for non-OPTIONS requests
app.UseCors("AllowAngularApp");

// Return JSON for all errors so the frontend never gets HTML (avoids "Http failure during parsing")
app.UseExceptionHandler(errApp =>
{
    errApp.Run(async ctx =>
    {
        ctx.Response.StatusCode = StatusCodes.Status500InternalServerError;
        ctx.Response.ContentType = "application/json";
        var ex = ctx.Features.Get<Microsoft.AspNetCore.Diagnostics.IExceptionHandlerFeature>()?.Error;
        await ctx.Response.WriteAsJsonAsync(new { error = "Internal server error", message = ex?.Message ?? "An error occurred." });
    });
});

app.MapOpenApi();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();