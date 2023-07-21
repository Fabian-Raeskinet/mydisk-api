using Duende.IdentityServer.EntityFramework.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using MyDisks.Data;

namespace MyDisk.Migrations;

public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", true, true)
            .Build();

        var connectionString = args.Any() ? args[0] : configuration.GetConnectionString("DefaultConnection");
        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
        optionsBuilder.UseSqlServer(
            connectionString,
            b =>
            {
                b.MigrationsAssembly(GetType().Assembly.GetName().Name);
                b.CommandTimeout(600);
            });

        var dbContext = new ApplicationDbContext(optionsBuilder.Options);

        return dbContext;
    }
}

public class OperationalStoreOptionsMigrations :
    IOptions<OperationalStoreOptions>
{
    public OperationalStoreOptions Value => new()
    {
        DeviceFlowCodes = new TableConfiguration("DeviceCodes"),
        EnableTokenCleanup = false,
        PersistedGrants = new TableConfiguration("PersistedGrants"),
        TokenCleanupBatchSize = 100,
        TokenCleanupInterval = 3600
    };
}