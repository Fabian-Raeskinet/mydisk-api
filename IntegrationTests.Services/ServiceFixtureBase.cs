using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using MyDisks.Data;
using MyDisks.Domain.Disks;
using MyDisks.Services;

namespace IntegrationTests.Services;

public class ServiceFixtureBase : IDisposable
{
    protected ServiceProvider ServiceProvider { get; }

    public ServiceFixtureBase()
    {
        var services = new ServiceCollection();

        // services.AddScoped((s) => new Mock<IDiskRepository>().Object);
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false)
            .Build();
        
        services.AddApplicationServices();
        services.AddInfrastructureServices(configuration);

        ServiceProvider = services.BuildServiceProvider();
    }

    public void Dispose()
    {
        ServiceProvider.Dispose();
    }
}