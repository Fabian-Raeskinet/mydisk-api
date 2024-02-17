using FluentAssertions;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using MyDisks.Contracts.Disks;
using MyDisks.IntegrationTests.Services;
using MyDisks.Services.Disks;
using MyDisks.Tests.Services;

namespace MyDisks.Services.IntegrationTests;

public class GetAllDisksQueryHandlerFixture : ServiceFixtureBase
{
    [Theory]
    [AutoServiceData]
    public async Task Should(GetAllDisksQueryRequest request)
    {
        // Arrange
        var sut = ServiceProvider.GetRequiredService<IRequestHandler<GetAllDisksQueryRequest, IEnumerable<DiskResult>>>();
        
        // Act
        var act = await sut?.Handle(request, CancellationToken.None)!;

        act.Should().NotBeEmpty();
    }
}