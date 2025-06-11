using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace ProductsApi.Products.Entities
{
    [Index(nameof(Name), nameof(ColourName), IsUnique = true)]
    public class ProductEntity
    {
        public long Id { get; set; }

        [Required]
        public required string Name { get; set; }

        [Required]
        public required string ColourName { get; set; }

        [Required]
        public required int CurrentStock { get; set; }
    }
}
