using SuperMarket.Domain.Common;

namespace SuperMarket.Domain.Entities;

public class Product : BaseEntity
{
    public string Name { get; private set; }
    public decimal Price { get; private set; }
    public int Stock { get; private set; }
    public Guid CategoryId { get; private set; }

    private Product() { }

    public Product(string name, decimal price, int initialStock, Guid categoryId)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Product name is required");

        if (price <= 0)
            throw new ArgumentException("Price must be greater than zero");

        if (initialStock < 0)
            throw new ArgumentException("Stock cannot be negative");

        Id = Guid.NewGuid();
        Name = name;
        Price = price;
        Stock = initialStock;
        CategoryId = categoryId;
    }

    public void UpdateDetails(string name, decimal price, Guid categoryId)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Product name is required");

        if (price <= 0)
            throw new ArgumentException("Price must be greater than zero");

        Name = name;
        Price = price;
        CategoryId = categoryId;
    }

    public void AddStock(int quantity)
    {
        if (quantity <= 0)
            throw new ArgumentException("Quantity must be greater than zero");

        Stock += quantity;
    }

    public void ReduceStock(int quantity)
    {
        if (quantity <= 0)
            throw new ArgumentException("Quantity must be greater than zero");

        if (quantity > Stock)
            throw new InvalidOperationException("Insufficient stock");

        Stock -= quantity;
    }
}