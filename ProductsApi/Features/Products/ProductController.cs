using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductsApi.Products.DTOs;
using ProductsApi.Products.Models;
using ProductsApi.Products.Service;

namespace ProductsApi.Controller
{
    [ApiController]
    [Route("products")]
    public class ProductsController : ControllerBase
    {
        private readonly ProductsService _productsService;

        public ProductsController(ProductsService productsService)
        {
            _productsService = productsService;
        }

        [Authorize(Policy = "AdminOnly")]
        [HttpPost]
        public async Task<ActionResult<Product>> CreateProduct(
            [FromBody] ProductWriteDTO productWriteRequest
        )
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var createProductResult = await _productsService.CreateNewProduct(productWriteRequest);
            if (!createProductResult.IsSuccess)
                return BadRequest(createProductResult.Errors);

            return Created();
        }

        [Authorize]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductReadDTO>>> GetProducts(string? colour)
        {
            // Allow the colour to be an optional query string
            var productsResult = await _productsService.ListProducts(colour);
            if (!productsResult.IsSuccess)
                return BadRequest(productsResult.Errors);

            var products = productsResult.Value;
            var productsReadDTOs = products.Select(product => product.ToReadDTO());
            return Ok(productsReadDTOs);
        }
    }
}
