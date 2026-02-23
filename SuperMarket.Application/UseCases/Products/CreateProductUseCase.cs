using SuperMarket.Application.Interfaces;
using SuperMarket.Domain.Entities;

namespace SuperMarket.Application.UseCases.Products;

public class CreateProductUseCase
{
    private readonly IProductRepository _productRepository;

    public CreateProductUseCase(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<Product> ExecuteAsync(
        string name,
        decimal price,
        int stock,
        Guid categoryId)
    {
        var nameTrimmed = name?.Trim() ?? string.Empty;
        if (await _productRepository.ExistsByNameAndCategoryAsync(nameTrimmed, categoryId))
            throw new InvalidOperationException("A product with this name already exists in this category.");

        var product = new Product(nameTrimmed, price, stock, categoryId);
        await _productRepository.AddAsync(product);
        return product;
    }
}
