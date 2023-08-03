using FluentAssertions;
using MediatR;
using Moq;
using MyDisks.Domain.Entities;
using MyDisks.Services.Disks;
using MyDisks.Tests.Services;
using MyDisks.Tests.Utils;

namespace MyDisks.Services.Tests.Disks;

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
                && disk.ReleaseDate == request.ReleaseDate)));
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