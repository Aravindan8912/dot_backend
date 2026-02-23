using SuperMarket.Application.Interfaces;
using SuperMarket.Domain.Entities;

namespace SuperMarket.Application.UseCases.Payments;

public class CreatePaymentUseCase{
    private readonly IPaymentRepository _paymentRepository;

    public CreatePaymentUseCase(IPaymentRepository paymentRepository){
        _paymentRepository = paymentRepository;
    }

    public async Task<Guid> ExecuteAsync(Guid orderId, decimal amount, string paymentMethod){
        var payment = new Payment(orderId, amount, paymentMethod);
        await _paymentRepository.AddAsync(payment);
        return payment.Id;
    }
}