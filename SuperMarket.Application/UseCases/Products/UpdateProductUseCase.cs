using SuperMarket.Application.Interfaces;

namespace SuperMarket.Application.UseCases.Products;

public class UpdateProductUseCase
{
    private readonly IProductRepository _productRepository;

    public UpdateProductUseCase(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task ExecuteAsync(Guid id, string name, decimal price, Guid categoryId)
    {
        var product = await _productRepository.GetByIdAsync(id);
        if (product == null)
            throw new Exception("Product not found");
        product.UpdateDetails(name, price, categoryId);
        await _productRepository.UpdateAsync(product);
    }
}
