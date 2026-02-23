using SuperMarket.Domain.Entities;

namespace SuperMarket.Application.Interfaces;

public interface IAddressRepository{
    Task AddAsync(Address address);
    Task<List<Address>> GetByCustomerIdAsync(Guid customerId);
    Task UpdateAsync(Address address);
    Task DeleteAsync(Address address);
}