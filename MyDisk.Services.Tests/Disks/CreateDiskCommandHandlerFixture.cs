using FluentAssertions;
using MediatR;
using Moq;
using MyDisk.Domain.Entities;
using MyDisk.Services.Disks;
using MyDisk.Tests.Services;
using MyDisk.Tests.Utils;

namespace MyDisk.Services.Tests.Disks;

public class CreateDiskCommandHandlerFixture
{
    [Theory]
    [AutoServiceData]
    public async Task Should_CreateDiskAsync
    (
        CreateDiskCommandRequest request,
        CreateDiskCommandHandler sut
    )
    {
        // Act
        var act = await sut.Handle(request, CancellationToken.None);

        // Assert
        sut.DiskRepository.AsMock()
            .Verify(x => x.CreateDiskAsync(It.Is<Disk>(disk =>
                disk.Name == request.Name
                && disk.ReleaseDate == request.ReleaseDate)), Times.Once);
    }

    [Theory]
    [AutoServiceData]
    public async Task Should_Returns_Unit_Object
    (
        CreateDiskCommandRequest request,
        CreateDiskCommandHandler sut
    )
    {
        // Act
        var act = await sut.Handle(request, CancellationToken.None);

        // Assert
        act.Should().BeOfType<Unit>();
    }
}