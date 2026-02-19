using Microsoft.AspNetCore.Mvc;
using SuperMarket.API.Requests;
using SuperMarket.Application.DTOs;
using SuperMarket.Application.Interfaces;
using SuperMarket.Application.UseCases.Orders;

namespace SuperMarket.API.Controllers;

[ApiController]
[Route("api/orders")]
public class OrdersController : ControllerBase
{
    private readonly IOrderRepository _orderRepository;
    private readonly CreateOrderUseCase _createOrderUseCase;

    public OrdersController(IOrderRepository orderRepository, CreateOrderUseCase createOrderUseCase)
    {
        _orderRepository = orderRepository;
        _createOrderUseCase = createOrderUseCase;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var orders = await _orderRepository.GetAllAsync();
        return Ok(orders);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        try
        {
            var order = await _orderRepository.GetByIdAsync(id);
            return Ok(order);
        }
        catch (InvalidOperationException)
        {
            return NotFound();
        }
    }

    [HttpPost]
    public async Task<IActionResult> CreateOrder(CreateOrderRequest request)
    {
        var dto = new CreateOrderDto(request.CustomerId, request.Items.Select(item => new CreateOrderItemDto(item.ProductId, item.Quantity)).ToList());
        var order = await _createOrderUseCase.ExecuteAsync(dto);
        return Ok(order);
    }
}