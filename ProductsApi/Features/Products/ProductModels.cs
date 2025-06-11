using System.Drawing;
using ProductsApi.Common.Models;
using ProductsApi.Products.DTOs;
using ProductsApi.Products.Entities;

namespace ProductsApi.Products.Models
{
    public class ProductColour
    {
        private Color _colour;

        public static Result<ProductColour> FromName(string colourName)
        {
            if (colourName is null)
                return Result<ProductColour>.Failure(["A colour name must be provided"]);

            var colourValue = Color.FromName(colourName);
            if (!colourValue.IsKnownColor)
                return Result<ProductColour>.Failure([$"{colourName} is not a valid colour"]);

            var productColour = new ProductColour { _colour = colourValue };

            return Result<ProductColour>.Success(productColour);
        }

        public string ColourName()
        {
            return _colour.Name;
        }
    }

    public class Product
    {
        public required string Name { get; set; }

        public required ProductColour Colour { get; set; }

        public required int CurrentStock { get; set; }

        // Realisitically I would probably use a seperate mapping namepsace to go from DTO -> Domain -> Entity
        // and back again but given the problem is trivial I'll stick to Locality of Behaviour
  
        private static Result<Product> CreateProduct(
            string productName,
            string colourName,
            int currentStock
        )
        {
            var productColourResult = ProductColour.FromName(colourName);
            if (!productColourResult.IsSuccess)
                return Result<Product>.Failure(productColourResult.Errors);

            if (currentStock <= 0)
                return Result<Product>.Failure(["Stock has to be greater than 0"]);

            var productColour = productColourResult.Value;

            var product = new Product
            {
                Name = productName,
                Colour = productColour,
                CurrentStock = currentStock,
            };

            return Result<Product>.Success(product);
        }

        public static Result<Product> FromWriteDTO(ProductWriteDTO productWrite)
        {
            var productCreateResult = CreateProduct(
                productWrite.Name,
                productWrite.Colour,
                productWrite.CurrentStock
            );
            if (!productCreateResult.IsSuccess)
                return Result<Product>.Failure(productCreateResult.Errors);
            return Result<Product>.Success(productCreateResult.Value);
        }

        public static Result<Product> FromEntity(ProductEntity productEntity)
        {
            var productCreateResult = CreateProduct(
                productEntity.Name,
                productEntity.ColourName,
                productEntity.CurrentStock
            );
            if (!productCreateResult.IsSuccess)
                return Result<Product>.Failure(productCreateResult.Errors);
            return Result<Product>.Success(productCreateResult.Value);
        }

        public ProductReadDTO ToReadDTO()
        {
            return new ProductReadDTO { Name = Name, Colour = Colour.ColourName() };
        }

        public ProductEntity ToEntity()
        {
            return new ProductEntity
            {
                Name = Name,
                ColourName = Colour.ColourName(),
                CurrentStock = CurrentStock,
            };
        }
    }
}
