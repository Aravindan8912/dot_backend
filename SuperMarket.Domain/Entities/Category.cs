using SuperMarket.Domain.Common;
namespace SuperMarket.Domain.Entities;

public class Category : BaseEntity{
    public string Name { get; private set; }

    private Category(){}

    public Category(string name)
    {
        Id = Guid.NewGuid();
        Name = name;
    }
    public void Update(string name)
    {
        Name = name;
    }
}