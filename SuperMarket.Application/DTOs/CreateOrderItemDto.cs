namespace SuperMarket.Application.DTOs;


public record CreateOrderItemDto(
    Guid ProductId,
    int Quantity
);