using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using ProductsApi.Data;

public class DatabaseTestFixture : IAsyncLifetime
{
    public AppDbContext DbContext { get; private set; } = null!;
    private IDbContextTransaction _transaction = null!;

    private const string ConnectionString =
        "Server=sqlserver;Database=MyDatabase;User Id=sa;Password=LocalDev!;TrustServerCertificate=True";

    public async Task InitializeAsync()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlServer(ConnectionString)
            .Options;

        DbContext = new AppDbContext(options);

        await DbContext.Database.MigrateAsync();
        await DbContext.Database.ExecuteSqlRawAsync("DELETE FROM Products"); // Dont run this against a remote db lol

        _transaction = await DbContext.Database.BeginTransactionAsync();
    }

    public async Task DisposeAsync()
    {
        // Roll back after each test
        await _transaction.RollbackAsync();
        await _transaction.DisposeAsync();
        await DbContext.DisposeAsync();
    }
}
