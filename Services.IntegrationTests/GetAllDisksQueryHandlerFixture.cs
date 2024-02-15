using FluentAssertions;
using IntegrationTests.Services;
using MediatR;
using MyDisks.Contracts.Disks;
using MyDisks.Services;
using MyDisks.Services.Disks;
using MyDisks.Tests.Services;

namespace Services.IntegrationTests;

public class GetAllDisksQueryHandlerFixture : ServiceFixtureBase
{
    [Theory]
    [AutoServiceData]
    public async Task Should(GetAllDisksQueryRequest request)
    {
        // Arrange
        var sut = ServiceProvider.GetService(typeof(IRequestHandler<GetAllDisksQueryRequest, IEnumerable<DiskResult>>)) as IRequestHandler<GetAllDisksQueryRequest, IEnumerable<DiskResult>>;
        
        // Act
        var act = await sut?.Handle(request, CancellationToken.None)!;

        act.Should().NotBeEmpty();
    }
}