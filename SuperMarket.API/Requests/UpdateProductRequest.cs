namespace SuperMarket.API.Requests;

public record UpdateProductRequest(
    string Name,
    decimal Price,
    Guid CategoryId
);

