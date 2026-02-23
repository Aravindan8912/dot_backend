using SuperMarket.Application.Interfaces;
using SuperMarket.Domain.Entities;
using SuperMarket.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace SuperMarket.Infrastructure.Repositories;

public class DeliveryRepository : IDeliveryRepository
{
    private readonly AppDbContext _context;

    public DeliveryRepository(AppDbContext context) => _context = context;

    public async Task AddAsync(Delivery delivery)
    {
        await _context.Deliveries.AddAsync(delivery);
        await _context.SaveChangesAsync();
    }

    public async Task<Delivery?> GetByOrderIdAsync(Guid orderId)
        => await _context.Deliveries.FirstOrDefaultAsync(d => d.OrderId == orderId);

    public async Task UpdateAsync(Delivery delivery)
    {
        _context.Deliveries.Update(delivery);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Delivery delivery)
    {
        _context.Deliveries.Remove(delivery);
        await _context.SaveChangesAsync();
    }
}
