using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using MyDisks.Api;
using MyDisks.Domain;

namespace IntegrationTests.Api;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    public CustomWebApplicationFactory()
    {
        DiskRepositoryMock = new Mock<IDiskRepository>();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        base.ConfigureWebHost(builder);
        builder.ConfigureServices(
            services => { services.AddSingleton(DiskRepositoryMock.Object); });
    }

    public Mock<IDiskRepository> DiskRepositoryMock { get; }
}