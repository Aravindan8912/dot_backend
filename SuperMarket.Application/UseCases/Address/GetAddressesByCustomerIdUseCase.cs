using SuperMarket.Application.Interfaces;
using SuperMarket.Domain.Entities;

namespace SuperMarket.Application.UseCases.Address;

public class GetAddressesByCustomerIdUseCase
{
    private readonly IAddressRepository _addressRepository;

    public GetAddressesByCustomerIdUseCase(IAddressRepository addressRepository)
    {
        _addressRepository = addressRepository;
    }

    public async Task<List<SuperMarket.Domain.Entities.Address>> ExecuteAsync(Guid customerId)
        => await _addressRepository.GetByCustomerIdAsync(customerId);
}
