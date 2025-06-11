using Microsoft.AspNetCore.Identity;

namespace ProductsApi.Auth.Models
{
    public class ProductsApiUser : IdentityUser
    {
        public bool IsAdmin { get; set; }
    }
}
