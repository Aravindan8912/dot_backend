using SuperMarket.Domain.Common;
using SuperMarket.Domain.Enums;

namespace SuperMarket.Domain.Entities;

public class Payment : BaseEntity
{
    public Guid OrderId { get; private set; }
    public decimal Amount { get; private set; }
    public PaymentStatus Status { get; private set; }
    public string PaymentMethod { get; private set; } = null!;

    private Payment() { }

    public Payment(Guid orderId, decimal amount, string paymentMethod)
    {
        Id = Guid.NewGuid();
        OrderId = orderId;
        Amount = amount;
        PaymentMethod = paymentMethod;
        Status = PaymentStatus.Pending;
    }   

    public void MarkPaid(){
        Status = PaymentStatus.Paid;
    }

    public void MarkFailed(){
        Status = PaymentStatus.Failed;
    }

}