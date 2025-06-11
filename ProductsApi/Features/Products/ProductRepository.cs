using Microsoft.EntityFrameworkCore;
using ProductsApi.Common.DbHelpers;
using ProductsApi.Common.Models;
using ProductsApi.Data;
using ProductsApi.Products.Entities;

namespace ProductsApi.Products.Repository
{
    public class ProductRepository: IProductRepository
    {
        private readonly AppDbContext _context;

        public ProductRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Result<ProductEntity>> PersistNewProduct(ProductEntity product)
        {
            _context.Products.Add(product);
            try
            {
                await _context.SaveChangesAsync();
                return Result<ProductEntity>.Success(product);
            }
            catch (DbUpdateException ex)
            {
                // Say something than more useful than a 500 in the event we try to duplicate
                if (SqlExceptionHelper.IsUniqueConstraintViolation(ex))
                {
                    return Result<ProductEntity>.Failure(
                        ["A product with that Name and Colour already exists."]
                    );
                }

                // Give up after that
                throw;
            }
        }

        public async Task<Result<IEnumerable<ProductEntity>>> QueryProducts(string? colour)
        {
            IQueryable<ProductEntity> query = _context.Products;
            if (colour is not null)
            {
                query = query.Where(p => p.ColourName == colour);
            }

            var products = await query.ToListAsync();

            return Result<IEnumerable<ProductEntity>>.Success(products);
        }
    }
}
