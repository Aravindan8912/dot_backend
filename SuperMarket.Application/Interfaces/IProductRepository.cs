using SuperMarket.Domain.Entities;
namespace SuperMarket.Application.Interfaces;

public interface IProductRepository{
    Task<Product?> GetByIdAsync(Guid id);
    Task<IEnumerable<Product>> GetAllAsync();
    Task AddAsync(Product product);
    Task UpdateAsync(Product product);
    Task DeleteAsync(Product product);
}