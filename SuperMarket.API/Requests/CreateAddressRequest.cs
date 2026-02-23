namespace SuperMarket.API.Requests;

public record CreateAddressRequest(
    Guid CustomerId,
    string AddressLine1,
    string? AddressLine2,
    string City,
    string State,
    string ZipCode,
    string Country
);
