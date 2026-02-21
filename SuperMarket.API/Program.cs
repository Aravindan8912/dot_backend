using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SuperMarket.Infrastructure;
using SuperMarket.Infrastructure.Persistence;
using SuperMarket.Application.UseCases.Categories;
using SuperMarket.Application.UseCases.Customers;
using SuperMarket.Application.UseCases.Products;
using SuperMarket.Application.UseCases.Orders;
using SuperMarket.Application.UseCases.Auth;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();

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
    db.Database.Migrate();
    if (app.Environment.IsDevelopment())
        await scope.ServiceProvider.SeedDefaultAdminIfNeededAsync();
}

app.MapOpenApi();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();