using System.Data;
using Microsoft.Data.SqlClient;
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

    private SqlConnection SqlConnection
    {
        get
        {
            var dbContext = ServiceProvider.GetService<ApplicationDbContext>();
            var sqlConnection = dbContext?.Database.GetDbConnection() as SqlConnection;
            if (sqlConnection is { State: ConnectionState.Closed })
                sqlConnection.Open();
            return sqlConnection!;
        }
    }

    public void Dispose()
    {
        if (SqlConnection.State == ConnectionState.Open)
            SqlConnection.Close();

        SqlConnection.Dispose();
        ServiceProvider.Dispose();
    }
}