using FluentAssertions;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using MyDisks.Data;
using MyDisks.Domain.Disks;
using MyDisks.IntegrationTests.Services;
using MyDisks.Services.Disks;
using MyDisks.Tests.Services;

namespace MyDisks.Services.IntegrationTests.Disks;

public class CreateDiskCommandHandlerFixture : ServiceFixtureBase
{
    public CreateDiskCommandHandlerFixture()
    {
        DiskRepository = ServiceProvider.GetRequiredService<IDiskRepository>();
    }

    private IDiskRepository DiskRepository { get; }

    [Theory]
    [AutoServiceData]
    public async Task Should_Create_Disk(Name diskName)
    {
        // Arrange
        var request = new CreateDiskCommandRequest
        {
            Name = diskName,
            ReleaseDate = DateTime.Now
        };
        var sut = ServiceProvider.GetRequiredService<IRequestHandler<CreateDiskCommandRequest>>();

        // Act
        await sut?.Handle(request, CancellationToken.None)!;

        // Assert
        var disk = await DiskRepository.GetDiskByFilterAsync(x => x.Name == request.Name);
        disk.Should().NotBeNull();
    }
}