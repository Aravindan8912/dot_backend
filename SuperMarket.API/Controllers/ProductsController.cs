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
    private readonly ICategoryRepository _categoryRepository;
    private readonly CreateProductUseCase _createProductUseCase;
    private readonly UpdateProductUseCase _updateProductUseCase;

    public ProductsController(
        IProductRepository productRepository,
        ICategoryRepository categoryRepository,
        CreateProductUseCase createProductUseCase,
        UpdateProductUseCase updateProductUseCase)
    {
        _productRepository = productRepository;
        _categoryRepository = categoryRepository;
        _createProductUseCase = createProductUseCase;
        _updateProductUseCase = updateProductUseCase;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var products = (await _productRepository.GetAllAsync()).ToList();
        var categories = (await _categoryRepository.GetAllAsync()).ToDictionary(c => c.Id, c => c.Name);
        var result = products.Select(p => new
        {
            id = p.Id,
            name = p.Name,
            price = p.Price,
            stock = p.Stock,
            categoryId = p.CategoryId,
            category = new { name = categories.GetValueOrDefault(p.CategoryId, string.Empty) }
        });
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var product = await _productRepository.GetByIdAsync(id);
        if (product == null)
            return NotFound();
        var category = await _categoryRepository.GetByIdAsync(product.CategoryId);
        return Ok(new
        {
            id = product.Id,
            name = product.Name,
            price = product.Price,
            stock = product.Stock,
            categoryId = product.CategoryId,
            category = new { name = category?.Name ?? string.Empty }
        });
    }

    [HttpPost]
    public async Task<IActionResult> CreateProduct(CreateProductRequest request)
    {
        try
        {
            var product = await _createProductUseCase.ExecuteAsync(request.Name, request.Price, request.Stock, request.CategoryId);
            var category = await _categoryRepository.GetByIdAsync(product.CategoryId);
            return Ok(new
            {
                id = product.Id,
                name = product.Name,
                price = product.Price,
                stock = product.Stock,
                categoryId = product.CategoryId,
                category = new { name = category?.Name ?? string.Empty }
            });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateProduct(Guid id, UpdateProductRequest request)
    {
        try
        {
            var product = await _updateProductUseCase.ExecuteAsync(id, request.Name, request.Price, request.CategoryId);
            var category = await _categoryRepository.GetByIdAsync(product.CategoryId);
            return Ok(new
            {
                id = product.Id,
                name = product.Name,
                price = product.Price,
                stock = product.Stock,
                categoryId = product.CategoryId,
                category = new { name = category?.Name ?? string.Empty }
            });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }
}