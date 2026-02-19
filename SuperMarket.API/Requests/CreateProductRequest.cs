namespace SuperMarket.API.Requests;

public record CreateProductRequest(
    string Name,
    decimal Price,
    int Stock,
    Guid CategoryId
);