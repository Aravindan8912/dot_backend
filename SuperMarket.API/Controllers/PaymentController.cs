using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SuperMarket.API.Requests;
using SuperMarket.Application.UseCases.Payments;

namespace SuperMarket.API.Controllers;

[ApiController]
[Route("api/payments")]
[Authorize(Roles = "Admin,User")]
public class PaymentController : ControllerBase
{
    private readonly CreatePaymentUseCase _createUseCase;
    private readonly GetPaymentByOrderIdUseCase _getByOrderIdUseCase;

    public PaymentController(CreatePaymentUseCase createUseCase, GetPaymentByOrderIdUseCase getByOrderIdUseCase)
    {
        _createUseCase = createUseCase;
        _getByOrderIdUseCase = getByOrderIdUseCase;
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreatePaymentRequest request)
    {
        var id = await _createUseCase.ExecuteAsync(request.OrderId, request.Amount, request.PaymentMethod);
        return Ok(new { id, message = "Payment created successfully" });
    }

    [HttpGet]
    public async Task<IActionResult> GetByOrderId(Guid orderId)
    {
        var payment = await _getByOrderIdUseCase.ExecuteAsync(orderId);
        if (payment is null)
            return NotFound();
        return Ok(payment);
    }
}