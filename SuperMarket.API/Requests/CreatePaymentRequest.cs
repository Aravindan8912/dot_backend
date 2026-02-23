namespace SuperMarket.API.Requests;

public record CreatePaymentRequest(
    Guid OrderId,
    decimal Amount,
    string PaymentMethod
);
