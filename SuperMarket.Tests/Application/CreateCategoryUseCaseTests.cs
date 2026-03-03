using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SuperMarket.Application.Interfaces;
using SuperMarket.Application.UseCases.Categories;
using SuperMarket.Domain.Entities;

namespace SuperMarket.Tests.Application;

[TestClass]
public class CreateCategoryUseCaseTests
{
    private Mock<ICategoryRepository> _categoryRepositoryMock = null!;
    private CreateCategoryUseCase _sut = null!;

    [TestInitialize]
    public void Setup()
    {
        _categoryRepositoryMock = new Mock<ICategoryRepository>();
        _sut = new CreateCategoryUseCase(_categoryRepositoryMock.Object);
    }

    [TestMethod]
    public async Task ExecuteAsync_ValidName_CreatesAndReturnsCategory()
    {
        _categoryRepositoryMock
            .Setup(x => x.AddAsync(It.IsAny<Category>()))
            .Returns(Task.CompletedTask);

        var result = await _sut.ExecuteAsync("Dairy");

        Assert.IsNotNull(result);
        Assert.AreEqual("Dairy", result.Name);
        Assert.AreNotEqual(Guid.Empty, result.Id);
        _categoryRepositoryMock.Verify(x => x.AddAsync(It.Is<Category>(c => c.Name == "Dairy")), Times.Once);
    }
}
