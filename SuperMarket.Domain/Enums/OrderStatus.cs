namespace SuperMarket.Domain.Enums;

public enum OrderStatus
{
    Pending = 0,
    PendingPayment = 1,
    Paid = 2,
    Confirmed = 2, // same as Paid for Order workflow
    PreparedForDelivery = 3,
    OutForDelivery = 4,
    Shipped = 4, // same as OutForDelivery
    Delivered = 5,
    Cancelled = 6
}
