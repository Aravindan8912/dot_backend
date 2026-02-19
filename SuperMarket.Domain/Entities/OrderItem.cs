using SuperMarket.Domain.Common;

namespace SuperMarket.Domain.Entities;

public class OrderItem : BaseEntity{
    public Guid OrderId { get; private set; }
    public Guid ProductId { get; private set; }
    public decimal Price { get; private set; }
    public int Quantity { get; private set; }

    private OrderItem() { }

    internal OrderItem(Guid orderId, Guid productId, decimal price, int quantity){
        if(quantity <= 0)
            throw new ArgumentException("Quantity must be greater than zero");
            
        Id = Guid.NewGuid();
        OrderId = orderId;
        ProductId = productId;
        Price = price;
        Quantity = quantity;
    }
}