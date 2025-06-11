using ProductsApi.Common.Models;
using ProductsApi.Products.Entities;

namespace ProductsApi.Products.Repository
{
    public interface IProductRepository
    {
        Task<Result<ProductEntity>> PersistNewProduct(ProductEntity product);
        Task<Result<IEnumerable<ProductEntity>>> QueryProducts(string? colour);
    }
}