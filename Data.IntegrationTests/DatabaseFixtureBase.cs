using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using MyDisks.Tests.Utils;

namespace MyDisks.Data.IntegrationTests;

public class DatabaseFixtureBase : IDisposable
{
    protected DatabaseFixtureBase()
    {
        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
        optionsBuilder.UseSqlServer(JsonSettings.XUnit.GetConnectionString("SqlServerConnection"));

        DbContext = CreateApplicationDbContext(optionsBuilder);

        DbContext.Database.OpenConnection();
        DbContextTransaction = DbContext.Database.BeginTransaction();
    }

    protected ApplicationDbContext DbContext { get; }
    private IDbContextTransaction DbContextTransaction { get; }
    private ServiceProvider ServiceProvider { get; set; }

    public void Dispose()
    {
        DbContextTransaction.Rollback();
        DbContextTransaction.Dispose();
        DbContext.Dispose();
        ServiceProvider.Dispose();
    }

    private ApplicationDbContext CreateApplicationDbContext(
        DbContextOptionsBuilder<ApplicationDbContext> optionsBuilder)
    {
        var services = new ServiceCollection();

        services.AddSingleton(optionsBuilder.Options);
        services.AddScoped<ApplicationDbContext>();

        ServiceProvider = services.BuildServiceProvider()!;
        return ServiceProvider.GetRequiredService<ApplicationDbContext>();
    }
}