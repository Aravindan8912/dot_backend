namespace SuperMarket.API.Requests;

public record CreateCustomerRequest(
    string Name,
    string Email,
    string Phone,
    string? Password = null,
    string? Role = null
);
