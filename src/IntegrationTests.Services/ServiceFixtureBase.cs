using System.Data;
using System.Data.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using MyDisks.Data;
using MyDisks.Services;
using MyDisks.Services.Disks;

namespace MyDisks.IntegrationTests.Services;

public class ServiceFixtureBase : IDisposable
{
    protected ServiceProvider ServiceProvider { get; }

    protected ServiceFixtureBase()
    {
        var services = new ServiceCollection();

        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", false)
            .Build();

        services.AddApplicationServices();
        services.AddScoped((s) => new Mock<ILogger<NewDiskCreatedDomainEventHandler>>().Object);
        services.AddInfrastructureServices(configuration);

        ServiceProvider = services.BuildServiceProvider();
    }

    private DbConnection DbConnection
    {
        get
        {
            var dbContext = ServiceProvider.GetService<ApplicationDbContext>();
            var dbConnection = dbContext?.Database.GetDbConnection();
            if (dbConnection is { State: ConnectionState.Closed })
                dbConnection.Open();
            return dbConnection!;
        }
    }

    public void Dispose()
    {
        if (DbConnection.State == ConnectionState.Open)
            DbConnection.Close();

        DbConnection.Dispose();
        ServiceProvider.Dispose();
    }
}