using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SuperMarket.API.Requests;
using SuperMarket.Application.UseCases.Address;

namespace SuperMarket.API.Controllers;

[ApiController]
[Route("api/addresses")]
[Authorize(Roles = "Admin,User")]
public class AddressController : ControllerBase
{
    private readonly CreateAddressUseCase _createUseCase;
    private readonly GetAddressesByCustomerIdUseCase _getByCustomerIdUseCase;

    public AddressController(CreateAddressUseCase createUseCase, GetAddressesByCustomerIdUseCase getByCustomerIdUseCase)
    {
        _createUseCase = createUseCase;
        _getByCustomerIdUseCase = getByCustomerIdUseCase;
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateAddressRequest request)
    {
        var id = await _createUseCase.ExecuteAsync(
            request.CustomerId,
            request.AddressLine1,
            request.AddressLine2 ?? "",
            request.City,
            request.State,
            request.ZipCode,
            request.Country);
        return Ok(new { id, message = "Address created successfully" });
    }

    [HttpGet]
    public async Task<IActionResult> GetByCustomerId(Guid customerId)
    {
        var addresses = await _getByCustomerIdUseCase.ExecuteAsync(customerId);
        return Ok(addresses);
    }
}