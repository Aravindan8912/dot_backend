using SuperMarket.Application.Interfaces;
using SuperMarket.Domain.Entities;
using SuperMarket.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;


namespace SuperMarket.Infrastructure.Repositories;

public class OrderRepository : IOrderRepository{
    private readonly AppDbContext _context;

    public OrderRepository(AppDbContext context){
        _context = context;
    }

    public async Task<Order> GetByIdAsync(Guid id)
        => await _context.Orders.Include(o => o.Items).FirstOrDefaultAsync(o => o.Id == id)
            ?? throw new InvalidOperationException("Order not found");

    public async Task<IEnumerable<Order>> GetAllAsync()
        => await _context.Orders.Include(o => o.Items).ToListAsync();

    public async Task<Order> AddAsync(Order order)
    {
        _context.Orders.Add(order);
        await _context.SaveChangesAsync();
        return order;
    }

    public async Task<Order> UpdateAsync(Order order)
    {
        _context.Orders.Update(order);
        await _context.SaveChangesAsync();
        return order;
    }
}