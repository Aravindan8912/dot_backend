using SuperMarket.Domain.Common;

namespace SuperMarket.Domain.Entities;

public class Customer : BaseEntity
{
    public string Name { get; private set; } = null!;
    public string Email { get; private set; } = null!;
    public string Phone { get; private set; } = null!;
    public Guid? UserId { get; private set; }

    private Customer() { }

    public Customer(string name, string email, string phone, Guid? userId = null)
    {
        Id = Guid.NewGuid();
        Name = name;
        Email = email;
        Phone = phone;
        UserId = userId;
    }

    public void Update(string email, string phone)
    {
        Email = email;
        Phone = phone;
    }
}