namespace SuperMarket.Application.DTOs;

public record UpdateProductDto(
    Guid Id,
    string Name,
    decimal Price,
    Guid CategoryId
);