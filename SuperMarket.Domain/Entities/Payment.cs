public class Payment : BaseEntity{
    public Guid OrderId { get; private set; }
    public decimal Amount { get; private set; }
    public PaymentStatus Status { get; private set; }
    public string PaymentMethod { get; private set; }
    public string TransactionId { get; private set; }

    private Payment() { 
    }

    public Payment(Guid orderId, decimal amount, PaymentStatus status, string paymentMethod, string transactionId)
    {
        Id = Guid.NewGuid();
        OrderId = orderId;
        Amount = amount;
        Status = status;
        PaymentMethod = paymentMethod;
        TransactionId = transactionId;
    }   

    public void MarkPaid(){
        Status = PaymentStatus.Paid;
    }

    public void MarkFailed(){
        Status = PaymentStatus.Failed;
    }

}