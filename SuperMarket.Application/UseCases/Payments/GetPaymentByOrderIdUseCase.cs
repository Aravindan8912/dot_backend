using SuperMarket.Application.Interfaces;
using SuperMarket.Domain.Entities;

namespace SuperMarket.Application.UseCases.Payments;

public class GetPaymentByOrderIdUseCase
{
    private readonly IPaymentRepository _paymentRepository;

    public GetPaymentByOrderIdUseCase(IPaymentRepository paymentRepository)
    {
        _paymentRepository = paymentRepository;
    }

    public async Task<Payment?> ExecuteAsync(Guid orderId)
        => await _paymentRepository.GetByOrderIdAsync(orderId);
}
