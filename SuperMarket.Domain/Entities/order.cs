using SuperMarket.Domain.Common;
using SuperMarket.Domain.Enums;

namespace SuperMarket.Domain.Entities;

public class Order : BaseEntity
{
    public Guid CustomerId { get; private set; }
    public OrderStatus Status { get; private set; }
    public DateTime OrderDate { get; private set; }

    private readonly List<OrderItem> _items = new();
    public IReadOnlyCollection<OrderItem> Items => _items.AsReadOnly();

    private Order() { }

    public Order(Guid customerId)
    {
        if (customerId == Guid.Empty)
            throw new ArgumentException("CustomerId is required");

        Id = Guid.NewGuid();
        CustomerId = customerId;
        Status = OrderStatus.Pending;
        OrderDate = DateTime.UtcNow;
    }

    // Derived value (never stored manually)
    public decimal TotalAmount =>
        _items.Sum(i => i.Price * i.Quantity);

    public void AddItem(Product product, int quantity)
    {
        if (product is null)
            throw new ArgumentNullException(nameof(product));

        if (quantity <= 0)
            throw new ArgumentException("Quantity must be greater than zero");

        // Inventory rule
        product.ReduceStock(quantity);

        var orderItem = new OrderItem(
            orderId: Id,
            productId: product.Id,
            price: product.Price,
            quantity: quantity
        );

        _items.Add(orderItem);
    }

    public void RemoveItem(Guid productId)
    {
        var item = _items.FirstOrDefault(x => x.ProductId == productId);

        if (item is null)
            throw new InvalidOperationException("Item not found in order");

        _items.Remove(item);
    }

    public void Confirm()
    {
        if (Status != OrderStatus.Pending)
            throw new InvalidOperationException("Only pending orders can be confirmed");

        Status = OrderStatus.Confirmed;
    }

    public void Ship()
    {
        if (Status != OrderStatus.Confirmed)
            throw new InvalidOperationException("Only confirmed orders can be shipped");

        Status = OrderStatus.Shipped;
    }

    public void Deliver()
    {
        if (Status != OrderStatus.Shipped)
            throw new InvalidOperationException("Only shipped orders can be delivered");

        Status = OrderStatus.Delivered;
    }

    public void Cancel()
    {
        if (Status == OrderStatus.Delivered)
            throw new InvalidOperationException("Delivered orders cannot be cancelled");

        Status = OrderStatus.Cancelled;
    }
}