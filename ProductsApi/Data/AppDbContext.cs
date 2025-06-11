using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ProductsApi.Auth.Models;
using ProductsApi.Products.Entities;

namespace ProductsApi.Data;

public class AppDbContext : IdentityDbContext<ProductsApiUser>
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options) { }

    public DbSet<ProductEntity> Products { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
    }
}
