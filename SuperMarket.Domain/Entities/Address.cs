using SuperMarket.Domain.Common;

namespace SuperMarket.Domain.Entities;

public class Address : BaseEntity
{
    public Guid CustomerId { get; private set; }
    public string AddressLine1 { get; private set; } = null!;
    public string AddressLine2 { get; private set; } = null!;
    public string City { get; private set; } = null!;
    public string State { get; private set; } = null!;
    public string ZipCode { get; private set; } = null!;
    public string Country { get; private set; } = null!;

    private Address() { }

    public Address(Guid customerId, string addressLine1, string addressLine2, string city, string state, string zipCode, string country)
    {
        Id = Guid.NewGuid();
        CustomerId = customerId;
        AddressLine1 = addressLine1;
        AddressLine2 = addressLine2;
        City = city;
        State = state;
        ZipCode = zipCode;
        Country = country;
    }
}