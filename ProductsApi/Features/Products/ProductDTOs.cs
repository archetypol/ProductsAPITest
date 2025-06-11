using System.ComponentModel.DataAnnotations;

namespace ProductsApi.Products.DTOs
{
    public class ProductWriteDTO
    {
        [Required]
        public required string Name { get; set; }

        [Required]
        public required string Colour { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Stock has to be greater than 0")]
        public required int CurrentStock { get; set; }
    }

    public class ProductReadDTO
    {
        public required string Name { get; set; }

        public required string Colour { get; set; }
    }
}
