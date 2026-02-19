using SuperMarket.Domain.Entities;

namespace SuperMarket.Application.Interfaces;

public interface ICustomerRepository{
    Task<Customer> GetByIdAsync(Guid id);
    Task<IEnumerable<Customer>> GetAllAsync();
    Task<Customer> AddAsync(Customer customer);
    Task<Customer> UpdateAsync(Customer customer);
    Task<Customer> DeleteAsync(Customer customer);

}