using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SuperMarket.Application.Interfaces;
using SuperMarket.Application.UseCases.Products;
using SuperMarket.Domain.Entities;

namespace SuperMarket.Tests.Application;

[TestClass]
public class CreateProductUseCaseTests
{
    private Mock<IProductRepository> _productRepositoryMock = null!;
    private CreateProductUseCase _sut = null!;
    private static readonly Guid CategoryId = Guid.NewGuid();

    [TestInitialize]
    public void Setup()
    {
        _productRepositoryMock = new Mock<IProductRepository>();
        _sut = new CreateProductUseCase(_productRepositoryMock.Object);
    }

    [TestMethod]
    public async Task ExecuteAsync_ValidInputs_CreatesAndReturnsProduct()
    {
        _productRepositoryMock
            .Setup(x => x.ExistsByNameAndCategoryAsync("Milk", CategoryId))
            .ReturnsAsync(false);
        _productRepositoryMock
            .Setup(x => x.AddAsync(It.IsAny<Product>()))
            .Returns(Task.CompletedTask);

        var result = await _sut.ExecuteAsync("Milk", 2.99m, 100, CategoryId);

        Assert.IsNotNull(result);
        Assert.AreEqual("Milk", result.Name);
        Assert.AreEqual(2.99m, result.Price);
        Assert.AreEqual(100, result.Stock);
        Assert.AreEqual(CategoryId, result.CategoryId);
        _productRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Product>()), Times.Once);
    }

    [TestMethod]
    public async Task ExecuteAsync_TrimsProductName()
    {
        _productRepositoryMock
            .Setup(x => x.ExistsByNameAndCategoryAsync("Milk", CategoryId))
            .ReturnsAsync(false);
        _productRepositoryMock
            .Setup(x => x.AddAsync(It.IsAny<Product>()))
            .Returns(Task.CompletedTask);

        var result = await _sut.ExecuteAsync("  Milk  ", 2.99m, 100, CategoryId);

        Assert.AreEqual("Milk", result.Name);
    }

    [TestMethod]
    public async Task ExecuteAsync_DuplicateNameInCategory_ThrowsInvalidOperationException()
    {
        _productRepositoryMock
            .Setup(x => x.ExistsByNameAndCategoryAsync("Milk", CategoryId))
            .ReturnsAsync(true);

        await Assert.ThrowsExceptionAsync<InvalidOperationException>(() =>
            _sut.ExecuteAsync("Milk", 2.99m, 100, CategoryId));

        _productRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Product>()), Times.Never);
    }
}
