namespace ProductsApi.Tests;

using System.Threading.Tasks;
using Moq;
using ProductsApi.Common.Models;
using ProductsApi.Products.Entities;
using ProductsApi.Products.Models;
using ProductsApi.Products.Repository;
using ProductsApi.Products.Service;

public class ProductsServiceTest
{
    private readonly Mock<IProductRepository> _mockRepo;
    private readonly ProductsService _service;

    public ProductsServiceTest()
    {
        _mockRepo = new Mock<IProductRepository>();
        _service = new ProductsService(_mockRepo.Object);
    }

    [Fact]
    public async Task TestProductCreateSuccess()
    {
        var persistedProduct = new Product
        {
            Name = "Apple",
            Colour = ProductColour.FromName("Blue").Value!,
            CurrentStock = 1,
        };

        _mockRepo
            .Setup(r => r.PersistNewProduct(It.IsAny<ProductEntity>()))
            .ReturnsAsync(Result<ProductEntity>.Success(persistedProduct.ToEntity()));

        var productPersistedResult = await _service.CreateNewProduct(persistedProduct);
        Assert.True(productPersistedResult.IsSuccess);
    }

    [Fact]
    public async Task TestListProductsSuccess()
    {
        var productEntities = new List<ProductEntity>
        {
            new ProductEntity
            {
                Name = "Red Chair",
                ColourName = "Red",
                CurrentStock = 1,
            },
        };

        _mockRepo
            .Setup(r => r.QueryProducts("Red"))
            .ReturnsAsync(Result<IEnumerable<ProductEntity>>.Success(productEntities));

        var result = await _service.ListProducts("Red");

        Assert.True(result.IsSuccess);
        Assert.Single(result.Value);
        Assert.Equal("Red Chair", result.Value.First().Name);
    }

    [Fact]
    public async Task TestListProductsFailure()
    {
        // This will fail in mapping from entity to domain
        var productEntities = new List<ProductEntity>
        {
            new ProductEntity
            {
                Name = "Red Chair",
                ColourName = "Potato",
                CurrentStock = 1,
            },
        };

        _mockRepo
            .Setup(r => r.QueryProducts("Potato"))
            .ReturnsAsync(Result<IEnumerable<ProductEntity>>.Success(productEntities));

        var result = await _service.ListProducts("Potato");

        Assert.False(result.IsSuccess);
    }
}
