using Microsoft.VisualStudio.TestTools.UnitTesting;
using SuperMarket.Domain.Entities;

namespace SuperMarket.Tests.Domain;

[TestClass]
public class ProductTests
{
    private static readonly Guid CategoryId = Guid.NewGuid();

    [TestMethod]
    public void Constructor_ValidInputs_CreatesProduct()
    {
        var product = new Product("Milk", 2.99m, 100, CategoryId);

        Assert.AreEqual("Milk", product.Name);
        Assert.AreEqual(2.99m, product.Price);
        Assert.AreEqual(100, product.Stock);
        Assert.AreEqual(CategoryId, product.CategoryId);
        Assert.AreNotEqual(Guid.Empty, product.Id);
    }

    [TestMethod]
    [DataRow(null)]
    [DataRow("")]
    [DataRow("   ")]
    public void Constructor_EmptyOrWhitespaceName_ThrowsArgumentException(string? name)
    {
        Assert.ThrowsException<ArgumentException>(() =>
            new Product(name!, 1m, 0, CategoryId));
    }

    [TestMethod]
    public void Constructor_ZeroPrice_ThrowsArgumentException()
    {
        Assert.ThrowsException<ArgumentException>(() =>
            new Product("Milk", 0m, 10, CategoryId));
    }

    [TestMethod]
    public void Constructor_NegativePrice_ThrowsArgumentException()
    {
        Assert.ThrowsException<ArgumentException>(() =>
            new Product("Milk", -1m, 10, CategoryId));
    }

    [TestMethod]
    public void Constructor_NegativeStock_ThrowsArgumentException()
    {
        Assert.ThrowsException<ArgumentException>(() =>
            new Product("Milk", 1m, -1, CategoryId));
    }

    [TestMethod]
    public void UpdateDetails_ValidInputs_UpdatesProduct()
    {
        var product = new Product("Milk", 2.99m, 50, CategoryId);
        var newCategoryId = Guid.NewGuid();

        product.UpdateDetails("Organic Milk", 3.49m, newCategoryId);

        Assert.AreEqual("Organic Milk", product.Name);
        Assert.AreEqual(3.49m, product.Price);
        Assert.AreEqual(newCategoryId, product.CategoryId);
        Assert.AreEqual(50, product.Stock); // Stock unchanged
    }

    [TestMethod]
    [DataRow(null)]
    [DataRow("")]
    [DataRow("   ")]
    public void UpdateDetails_EmptyOrWhitespaceName_ThrowsArgumentException(string? name)
    {
        var product = new Product("Milk", 1m, 10, CategoryId);
        Assert.ThrowsException<ArgumentException>(() =>
            product.UpdateDetails(name!, 1m, CategoryId));
    }

    [TestMethod]
    public void AddStock_ValidQuantity_IncreasesStock()
    {
        var product = new Product("Milk", 1m, 10, CategoryId);
        product.AddStock(5);
        Assert.AreEqual(15, product.Stock);
    }

    [TestMethod]
    public void AddStock_ZeroOrNegative_ThrowsArgumentException()
    {
        var product = new Product("Milk", 1m, 10, CategoryId);
        Assert.ThrowsException<ArgumentException>(() => product.AddStock(0));
        Assert.ThrowsException<ArgumentException>(() => product.AddStock(-1));
    }

    [TestMethod]
    public void ReduceStock_ValidQuantity_DecreasesStock()
    {
        var product = new Product("Milk", 1m, 10, CategoryId);
        product.ReduceStock(3);
        Assert.AreEqual(7, product.Stock);
    }

    [TestMethod]
    public void ReduceStock_MoreThanAvailable_ThrowsInvalidOperationException()
    {
        var product = new Product("Milk", 1m, 10, CategoryId);
        Assert.ThrowsException<InvalidOperationException>(() => product.ReduceStock(11));
    }

    [TestMethod]
    public void ReduceStock_ZeroOrNegative_ThrowsArgumentException()
    {
        var product = new Product("Milk", 1m, 10, CategoryId);
        Assert.ThrowsException<ArgumentException>(() => product.ReduceStock(0));
        Assert.ThrowsException<ArgumentException>(() => product.ReduceStock(-1));
    }
}
