using SuperMarket.Application.Interfaces;
using SuperMarket.Domain.Entities;

namespace SuperMarket.Application.UseCases.Products;

public class UpdateProductUseCase
{
    private readonly IProductRepository _productRepository;

    public UpdateProductUseCase(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<Product> ExecuteAsync(Guid id, string name, decimal price, Guid categoryId)
    {
        var product = await _productRepository.GetByIdAsync(id);
        if (product == null)
            throw new Exception("Product not found");

        var nameTrimmed = name?.Trim() ?? string.Empty;
        if (await _productRepository.ExistsByNameAndCategoryExceptAsync(nameTrimmed, categoryId, id))
            throw new InvalidOperationException("A product with this name already exists in this category.");

        product.UpdateDetails(nameTrimmed, price, categoryId);
        await _productRepository.UpdateAsync(product);
        return product;
    }
}
