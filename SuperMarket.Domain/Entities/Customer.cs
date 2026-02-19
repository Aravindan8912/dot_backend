using SuperMarket.Domain.Common;
namespace SuperMarket.Domain.Entities;

public class Customer : BaseEntity{
    public string Name { get; private set; }
    public string Email { get; private set; }
    public string Phone { get; private set; }

    private Customer(){}

    public Customer(string name, string email, string phone)
    {
        Id = Guid.NewGuid();
        Name = name;
        Email = email;
        Phone = phone;
    }
    public void Update( string email, string phone)
    {
        Email = email;
        Phone = phone;
    }
}