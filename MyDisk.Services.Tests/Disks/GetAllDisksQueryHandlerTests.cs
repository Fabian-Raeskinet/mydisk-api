using AutoFixture.Xunit2;
using FluentAssertions;
using Moq;
using MyDisk.Contracts.Disks;
using MyDisk.Domain.Entities;
using MyDisk.Services.Disks;
using MyDisk.Tests.Services;
using MyDisk.Tests.Utils;

namespace MyDisk.Services.Tests.Disks;

public class GetAllDisksQueryHandlerTestsShould
{
    [Theory]
    [AutoServiceData]
    public async Task FindData
    (
        GetAllDisksQueryRequest request,
        List<Disk> disks,
        GetAllDisksQueryHandler sut
    )
    {
        // Arrange
        sut.DiskRepository.AsMock()
            .Setup(x => x.GetDisksAsync())
            .ReturnsAsync(disks);

        // Act
        var act = await sut.Handle(request, CancellationToken.None);

        // Assert
        act.Should().NotBeNull();
    }

    [Theory]
    [AutoServiceData]
    public async Task NotFindData
    (
        GetAllDisksQueryRequest request,
        List<Disk> disks,
        GetAllDisksQueryHandler sut
    )
    {
        // Arrange
        sut.DiskRepository.AsMock()
            .Setup(x => x.GetDisksAsync()).ReturnsAsync(disks);

        // Act
        var act = await sut.Handle(request,
            CancellationToken.None);

        // Assert
        act.Should().BeEmpty();
    }
}