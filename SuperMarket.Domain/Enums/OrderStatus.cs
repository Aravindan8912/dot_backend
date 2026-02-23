namespace SuperMarket.Domain.Enums;

public enum OrderStatus{
    PendingPayment=1,
    Paid=2,
    PreparedForDelivery=3,
    OutForDelivery=4,
    Delivered=5,
    Cancelled=6
}
