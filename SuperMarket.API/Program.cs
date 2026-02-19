using SuperMarket.Infrastructure;
using SuperMarket.Application.UseCases.Categories;
using SuperMarket.Application.UseCases.Customers;
using SuperMarket.Application.UseCases.Products;
using SuperMarket.Application.UseCases.Orders;


var builder=WebApplication.CreateBuilder(args);

//add service

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi(); // Replaces Swashbuckle (not yet compatible with .NET 10)

//Register Infrastructure

builder.Services.AddInfrastructure(builder.Configuration);

//register UseCase

builder.Services.AddScoped<CreateCategoryUseCase>();
builder.Services.AddScoped<CreateCustomerUseCase>();
builder.Services.AddScoped<CreateProductUseCase>();
builder.Services.AddScoped<CreateOrderUseCase>();
builder.Services.AddScoped<UpdateProductUseCase>();

var app = builder.Build();

app.MapOpenApi(); // OpenAPI spec at /openapi/v1.json
app.UseHttpsRedirection();
app.UseAuthorization();

app.MapControllers();

app.Run();