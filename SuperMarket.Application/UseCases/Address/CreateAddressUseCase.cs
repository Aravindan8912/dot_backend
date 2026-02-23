using SuperMarket.Application.Interfaces;
using SuperMarket.Domain.Entities;

namespace SuperMarket.Application.UseCases.Address;

public class CreateAddressUseCase
{
    private readonly IAddressRepository _addressRepository;

    public CreateAddressUseCase(IAddressRepository addressRepository)
    {
        _addressRepository = addressRepository;
    }

    public async Task<Guid> ExecuteAsync(Guid customerId, string addressLine1, string addressLine2, string city, string state, string zipCode, string country)
    {
        var address = new SuperMarket.Domain.Entities.Address(customerId, addressLine1, addressLine2, city, state, zipCode, country);
        await _addressRepository.AddAsync(address);
        return address.Id;
    }
}
