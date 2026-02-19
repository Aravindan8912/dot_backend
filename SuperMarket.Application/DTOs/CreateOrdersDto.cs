namespace SuperMarket.Application.DTOs;

public record CreateOrderDto(
    Guid CustomerId,
    List<CreateOrderItemDto> Items
);