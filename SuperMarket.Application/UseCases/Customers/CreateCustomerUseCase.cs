using SuperMarket.Application.Interfaces;
using SuperMarket.Domain.Entities;

namespace SuperMarket.Application.UseCases.Customers;

public class CreateCustomerUseCase
{
    private readonly ICustomerRepository _customerRepository;

    public CreateCustomerUseCase(ICustomerRepository customerRepository)
    {
        _customerRepository = customerRepository;
    }

    public async Task<Customer> ExecuteAsync(string name, string email, string phone)
    {
        var customer = new Customer(name, email, phone);
        await _customerRepository.AddAsync(customer);
        return customer;
    }
}
