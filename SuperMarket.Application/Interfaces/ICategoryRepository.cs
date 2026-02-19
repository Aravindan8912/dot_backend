using SuperMarket.Domain.Entities;

namespace SuperMarket.Application.Interfaces;

public interface ICategoryRepository{

    Task<IEnumerable<Category>> GetAllAsync();
    Task<Category?> GetByIdAsync(Guid id);
    Task AddAsync(Category category);
    Task UpdateAsync(Category category);
    Task DeleteAsync(Category category);
}