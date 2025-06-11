using Microsoft.EntityFrameworkCore;
using ProductsApi.Data;
using ProductsApi.Products.Entities;
using ProductsApi.Products.Repository;

namespace ProductsApi.Tests.Integration
{
    public class ProductRepositoryTests : IClassFixture<DatabaseTestFixture>
    {
        private readonly AppDbContext _db;
        private readonly ProductRepository _repo;

        public ProductRepositoryTests(DatabaseTestFixture fixture)
        {
            _db = fixture.DbContext;
            _repo = new ProductRepository(fixture.DbContext);
        }

        [Fact]
        public async Task PersistNewProduct_SavesToDatabase()
        {
            var productEntity = new ProductEntity
            {
                Name = "Apple",
                ColourName = "Green",
                CurrentStock = 5,
            };

            var persistResult = await _repo.PersistNewProduct(productEntity);

            Assert.True(persistResult.IsSuccess);

            var savedProduct = await _db.Products.FirstOrDefaultAsync(p =>
                p.Name == "Apple" && p.ColourName == "Green"
            );

            Assert.NotNull(savedProduct);
            Assert.Equal(5, savedProduct.CurrentStock);
        }

        [Fact]
        public async Task ListProductsByColour()
        {
            var blueProductEntity = new ProductEntity
            {
                Name = "Apple",
                ColourName = "Blue",
                CurrentStock = 5,
            };
            var greenProductEntity = new ProductEntity
            {
                Name = "Apple",
                ColourName = "Green",
                CurrentStock = 5,
            };

            await _repo.PersistNewProduct(blueProductEntity);
            await _repo.PersistNewProduct(greenProductEntity);

            var allProductsQueryResult = await _repo.QueryProducts(null);
            Assert.True(allProductsQueryResult.IsSuccess);
            Assert.Equal(2, allProductsQueryResult.Value.Count());

            var blueProductsQueryResult = await _repo.QueryProducts("Blue");
            Assert.True(blueProductsQueryResult.IsSuccess);
            Assert.Single(blueProductsQueryResult.Value);
        }
    }
}
