public class Delivery : BaseEntity{
    public Guid OrderId { get; private set; }
    public Guid CustomerId { get; private set; }
    public Guid AddressId { get; private set; }
    public Guid PaymentId { get; private set; }
    public DeliveryStatus Status { get; private set; }

    private Delivery() { }

    public Delivery(Guid orderId, Guid customerId, Guid addressId, Guid paymentId)
    {
        Id = Guid.NewGuid();
        OrderId = orderId;
        CustomerId = customerId;
        AddressId = addressId;
        PaymentId = paymentId;
        Status = DeliveryStatus.Pending;
    }   

    public void MarkOutForDelivery(){
        Status = DeliveryStatus.OutForDelivery;
    }

    public void MarkDelivered(){
        Status = DeliveryStatus.Delivered;
    }
}