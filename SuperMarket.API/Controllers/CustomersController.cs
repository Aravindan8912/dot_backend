using Microsoft.AspNetCore.Mvc;
using SuperMarket.API.Requests;
using SuperMarket.Application.Interfaces;
using SuperMarket.Application.UseCases.Customers;

namespace SuperMarket.API.Controllers;

[ApiController]
[Route("api/customers")]
public class CustomersController : ControllerBase
{
    private readonly ICustomerRepository _customerRepository;
    private readonly CreateCustomerUseCase _createCustomerUseCase;

    public CustomersController(ICustomerRepository customerRepository, CreateCustomerUseCase createCustomerUseCase)
    {
        _customerRepository = customerRepository;
        _createCustomerUseCase = createCustomerUseCase;
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateCustomerRequest request)
    {
        var customer = await _createCustomerUseCase.ExecuteAsync(request.Name, request.Email, request.Phone);
        return Ok(customer);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var customers = await _customerRepository.GetAllAsync();
        return Ok(customers);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        try
        {
            var customer = await _customerRepository.GetByIdAsync(id);
            return Ok(customer);
        }
        catch (InvalidOperationException)
        {
            return NotFound();
        }
    }
}
