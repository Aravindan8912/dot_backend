using SuperMarket.Application.Interfaces;
using SuperMarket.Domain.Entities;

namespace SuperMarket.Application.UseCases.Categories;

public class CreateCategoryUseCase
{
    private readonly ICategoryRepository _categoryRepository;

    public CreateCategoryUseCase(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public async Task<Category> ExecuteAsync(string name)
    {
        var category = new Category(name);
        await _categoryRepository.AddAsync(category);
        return category;
    }
}
