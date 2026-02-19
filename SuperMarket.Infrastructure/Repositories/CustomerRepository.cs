using SuperMarket.Application.Interfaces;
using SuperMarket.Domain.Entities;
using SuperMarket.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;


namespace SuperMarket.Infrastructure.Repositories;


public class CustomerRepository : ICustomerRepository{
    private readonly AppDbContext _context;

    public CustomerRepository(AppDbContext context){
        _context = context;
    }

    public async Task<Customer> GetByIdAsync(Guid id)
        => await _context.Customers.FindAsync(id)
            ?? throw new InvalidOperationException("Customer not found");

    public async Task<IEnumerable<Customer>> GetAllAsync()
    => await _context.Customers.ToListAsync();

    public async Task<Customer> AddAsync(Customer customer)
    {
        _context.Customers.Add(customer);
        await _context.SaveChangesAsync();
        return customer;
    }

    public async Task<Customer> UpdateAsync(Customer customer)
    {
        _context.Customers.Update(customer);
        await _context.SaveChangesAsync();
        return customer;
    }

    public async Task<Customer> DeleteAsync(Customer customer)
    {
        _context.Customers.Remove(customer);
        await _context.SaveChangesAsync();
        return customer;
    }
}