using ProductsApi.Common.Models;
using ProductsApi.Products.Models;
using ProductsApi.Products.DTOs;
using ProductsApi.Products.Repository;

namespace ProductsApi.Products.Service
{
    public class ProductsService
    {
        private readonly IProductRepository _repo;

        public ProductsService(IProductRepository repo)
        {
            _repo = repo;
        }

        public async Task<Result<Product>> CreateNewProduct(ProductWriteDTO productWriteRequest)
        {
            var productResult = Product.FromWriteDTO(productWriteRequest);
            if (!productResult.IsSuccess)
                return Result<Product>.Failure(productResult.Errors);

            var product = productResult.Value;

            var productPersistResult = await _repo.PersistNewProduct(product.ToEntity());
            if (!productPersistResult.IsSuccess)
                return Result<Product>.Failure(productPersistResult.Errors);

            return Result<Product>.Success(product);
        }
        public async Task<Result<Product>> CreateNewProduct(Product product)
        {
            var productPersistResult = await _repo.PersistNewProduct(product.ToEntity());
            if (!productPersistResult.IsSuccess)
                return Result<Product>.Failure(productPersistResult.Errors);

            return Result<Product>.Success(product);
        }

        public async Task<Result<IEnumerable<Product>>> ListProducts(string? colour)
        {
            if ((colour is not null) && !ProductColour.FromName(colour).IsSuccess)
                return Result<IEnumerable<Product>>.Failure(["Colour is Invalid"]);

            var productQueryResult = await _repo.QueryProducts(colour);
            if (!productQueryResult.IsSuccess)
                return Result<IEnumerable<Product>>.Failure(productQueryResult.Errors);

            var productsEntityMappingResults = productQueryResult
                .Value.Select(Product.FromEntity)
                .ToList();

            // wildly defensive by why not
            var mappingErrors = productsEntityMappingResults
                .Where(r => !r.IsSuccess)
                .SelectMany(r => r.Errors!)
                .ToList();

            if (mappingErrors.Any())
                return Result<IEnumerable<Product>>.Failure(mappingErrors);

            var products = productsEntityMappingResults
                .Where(r => r.IsSuccess)
                .Select(r => r.Value)
                .ToList();

            return Result<IEnumerable<Product>>.Success(products!);
        }
    }
}
