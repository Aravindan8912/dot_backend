using SuperMarket.Application.Interfaces;
using SuperMarket.Application.DTOs;
using SuperMarket.Domain.Entities;


namespace SuperMarket.Application.UseCases.Orders;

public class CreateOrderUseCase{
    private readonly IOrderRepository _orderRepository;
    private readonly ICustomerRepository _customerRepository;
    private readonly IProductRepository _productRepository;

    public CreateOrderUseCase(
        IOrderRepository orderRepository,
     ICustomerRepository customerRepository, 
     IProductRepository productRepository){
        _orderRepository = orderRepository;
        _customerRepository = customerRepository;
        _productRepository = productRepository;
    }

    public async Task<Order> ExecuteAsync(CreateOrderDto createOrderDto)
    {
        var customer = await _customerRepository.GetByIdAsync(createOrderDto.CustomerId);
        if(customer == null){
            throw new Exception("Customer not found");
        }
        var order = new Order(createOrderDto.CustomerId);
        foreach (var item in createOrderDto.Items)
        {
            var product = await _productRepository.GetByIdAsync(item.ProductId);
            if (product == null)
                throw new Exception("Product not found");
            order.AddItem(product, item.Quantity);
        }
        var created = await _orderRepository.AddAsync(order);
        return created;
    }
}