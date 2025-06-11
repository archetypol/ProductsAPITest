namespace ProductsApi.Tests;

using ProductsApi.Products.DTOs;
using ProductsApi.Products.Models;

// I mean there is basically nothing here worth testing
public class ProductsDomainTest
{
    [Fact]
    public void Create_Product_From_Write_DTO()
    {
        var writeDto = new ProductWriteDTO
        {
            Name = "Apple",
            Colour = "Blue",
            CurrentStock = 15,
        };

        var productCreateResult = Product.FromWriteDTO(writeDto);
        Assert.True(
            productCreateResult.IsSuccess,
            "We should of sucessfully created a domain model"
        );
    }

    [Fact]
    public void Product_Create_Fails_With_Invalid_Colour()
    {
        var writeDto = new ProductWriteDTO
        {
            Name = "Apple",
            Colour = "Potato",
            CurrentStock = 15,
        };

        var productCreateResult = Product.FromWriteDTO(writeDto);
        Assert.False(productCreateResult.IsSuccess, "Potato is not a colour");
    }
}
