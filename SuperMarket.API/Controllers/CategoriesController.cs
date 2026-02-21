using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SuperMarket.API.Requests;
using SuperMarket.Application.Interfaces;
using SuperMarket.Application.UseCases.Categories;

namespace SuperMarket.API.Controllers;

[ApiController]
[Route("api/categories")]
[Authorize(Roles = "Admin,User")]
public class CategoriesController : ControllerBase
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly CreateCategoryUseCase _createCategoryUseCase;

    public CategoriesController(ICategoryRepository categoryRepository, CreateCategoryUseCase createCategoryUseCase)
    {
        _categoryRepository = categoryRepository;
        _createCategoryUseCase = createCategoryUseCase;
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateCategoryRequest request)
    {
        var category = await _createCategoryUseCase.ExecuteAsync(request.Name);
        return Ok(category);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var categories = await _categoryRepository.GetAllAsync();
        return Ok(categories);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var category = await _categoryRepository.GetByIdAsync(id);
        if (category == null)
            return NotFound();
        return Ok(category);
    }
}
