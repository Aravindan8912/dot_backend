using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SuperMarket.API.Requests;
using SuperMarket.Application.DTOs;
using SuperMarket.Application.Interfaces;
using SuperMarket.Application.UseCases.Products;

namespace SuperMarket.API.Controllers;

[ApiController]
[Route("api/products")]
[Authorize(Roles = "Admin,User")]
public class ProductsController : ControllerBase
{
    private readonly IProductRepository _productRepository;
    private readonly CreateProductUseCase _createProductUseCase;
    private readonly UpdateProductUseCase _updateProductUseCase;

    public ProductsController(
        IProductRepository productRepository,
        CreateProductUseCase createProductUseCase,
        UpdateProductUseCase updateProductUseCase)
    {
        _productRepository = productRepository;
        _createProductUseCase = createProductUseCase;
        _updateProductUseCase = updateProductUseCase;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var products = await _productRepository.GetAllAsync();
        return Ok(products);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var product = await _productRepository.GetByIdAsync(id);
        if (product == null)
            return NotFound();
        return Ok(product);
    }

    [HttpPost]
    public async Task<IActionResult> CreateProduct( CreateProductRequest request){
        await _createProductUseCase.ExecuteAsync(request.Name, request.Price, request.Stock, request.CategoryId);
        return Ok("Product created successfully");
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateProduct(Guid id, UpdateProductRequest request){
        await _updateProductUseCase.ExecuteAsync(id, request.Name, request.Price, request.CategoryId);
        return Ok("Product updated successfully");
    }
}