using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using MyDisks.Api;
using MyDisks.Domain;
using MyDisks.Domain.Authors;
using MyDisks.Domain.Disks;

namespace MyDisks.IntegrationTests.Api;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    public CustomWebApplicationFactory()
    {
        AuthorRepositoryMock = new Mock<IAuthorRepository>();
        DiskRepositoryMock = new Mock<IDiskRepository>();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        base.ConfigureWebHost(builder);
        builder.ConfigureServices(
            services =>
            {
                services.AddSingleton(DiskRepositoryMock.Object);
                services.AddSingleton(AuthorRepositoryMock.Object);
            });
    }

    public Mock<IDiskRepository> DiskRepositoryMock { get; }
    public Mock<IAuthorRepository> AuthorRepositoryMock { get; }
}