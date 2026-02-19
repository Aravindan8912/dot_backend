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

    public async Task ExecuteAsync(
        string name,
        decimal price,
        int stock,
        Guid categoryId)
    {
        var product = new Product(name, price, stock, categoryId);
        await _productRepository.AddAsync(product);
    }
}
