using FluentAssertions;
using Moq;
using MyDisks.Contracts.Disks;
using MyDisks.Domain.Entities;
using MyDisks.Services.Disks;
using MyDisks.Tests.Services;
using MyDisks.Tests.Utils;

namespace MyDisks.Services.Tests.Disks;

public class GetAllDisksQueryHandlerFixture
{
    [Theory]
    [AutoServiceData]
    public async Task Should_Get_Disks
    (
        GetAllDisksQueryRequest request,
        GetAllDisksQueryHandler sut
    )
    {
        // Act
        var act = await sut.Handle(request, CancellationToken.None);

        // Assert
        sut.DiskRepository.AsMock()
            .Verify(_ => _.GetDisksAsync());
    }

    [Theory]
    [AutoServiceData]
    public async Task Should_Map_Disk
    (
        GetAllDisksQueryRequest request,
        IEnumerable<Disk> disks,
        GetAllDisksQueryHandler sut
    )
    {
        // Arrange
        sut.DiskRepository.AsMock()
            .Setup(_ => _.GetDisksAsync())
            .ReturnsAsync(disks.ToList());

        // Act
        var act = await sut.Handle(request, CancellationToken.None);

        // Assert
        sut.Mapper.AsMock()
            .Verify(_ => _.Map<IEnumerable<DiskResult>>(disks));
    }

    [Theory]
    [AutoServiceData]
    public async Task Should_Returns_Values
    (
        GetAllDisksQueryRequest request,
        IEnumerable<Disk> disks,
        IEnumerable<DiskResult> diskResults,
        GetAllDisksQueryHandler sut
    )
    {
        sut.DiskRepository.AsMock()
            .Setup(_ => _.GetDisksAsync())
            .ReturnsAsync(disks);

        sut.Mapper.AsMock()
            .Setup(_ => _.Map<IEnumerable<DiskResult>>(disks))
            .Returns(diskResults);

        // Act
        var act = await sut.Handle(request, CancellationToken.None);

        // Assert
        act.Should().NotBeEmpty();
    }

    [Theory]
    [AutoServiceData]
    public async Task Should_Returns_Empty_Values
    (
        GetAllDisksQueryRequest request,
        IEnumerable<Disk> disks,
        GetAllDisksQueryHandler sut
    )
    {
        sut.DiskRepository.AsMock()
            .Setup(_ => _.GetDisksAsync())
            .ReturnsAsync(disks);

        sut.Mapper.AsMock()
            .Setup(_ => _.Map<IEnumerable<DiskResult>>(disks))
            .Returns(Array.Empty<DiskResult>());

        // Act
        var act = await sut.Handle(request, CancellationToken.None);

        // Assert
        act.Should().BeEmpty();
    }
}