using SuperMarket.Application.Interfaces;
using SuperMarket.Domain.Entities;
using SuperMarket.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace SuperMarket.Infrastructure.Repositories;

public class ProductRepository : IProductRepository{
    private readonly AppDbContext _context;

    public ProductRepository(AppDbContext context){
        _context = context;
    }

    public async Task<Product?> GetByIdAsync(Guid id)
        => await _context.Products.FindAsync(id) 
        ?? throw new Exception("Product not found");

    public async Task<IEnumerable<Product>> GetAllAsync()
        => await _context.Products.ToListAsync();

    public async Task<bool> ExistsByNameAndCategoryAsync(string name, Guid categoryId)
    {
        var normalized = (name ?? string.Empty).Trim().ToLower();
        return await _context.Products
            .AnyAsync(p => p.Name.ToLower() == normalized && p.CategoryId == categoryId);
    }

    public async Task<bool> ExistsByNameAndCategoryExceptAsync(string name, Guid categoryId, Guid excludeProductId)
    {
        var normalized = (name ?? string.Empty).Trim().ToLower();
        return await _context.Products
            .AnyAsync(p => p.Id != excludeProductId && p.Name.ToLower() == normalized && p.CategoryId == categoryId);
    }

    public async Task AddAsync(Product product){
        _context.Products.Add(product);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Product product)
    {
        _context.Products.Update(product);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Product product)
    {
        _context.Products.Remove(product);
        await _context.SaveChangesAsync();
    }

}