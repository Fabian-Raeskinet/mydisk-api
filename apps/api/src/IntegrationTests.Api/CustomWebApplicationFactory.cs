using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using MyDisks.Api;
using MyDisks.Domain;
using MyDisks.Domain.Authors;
using MyDisks.Domain.Disks;
using MyDisks.Services;

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
        var unitOfWork = new Mock<IUnitOfWork>();
        unitOfWork.SetupGet(x => x.DiskRepository).Returns(DiskRepositoryMock.Object);
        unitOfWork.SetupGet(x => x.AuthorRepository).Returns(AuthorRepositoryMock.Object);
        builder.ConfigureTestServices(
            services =>
            {
                services.AddSingleton(DiskRepositoryMock.Object);
                services.AddSingleton(AuthorRepositoryMock.Object);
                services.AddSingleton(unitOfWork.Object);
                services.AddAuthentication(TestAuthenticationHandler.SchemeName)
                    .AddScheme<AuthenticationSchemeOptions, TestAuthenticationHandler>(
                        TestAuthenticationHandler.SchemeName, _ => { });
            });
    }

    public Mock<IDiskRepository> DiskRepositoryMock { get; }
    public Mock<IAuthorRepository> AuthorRepositoryMock { get; }
}
