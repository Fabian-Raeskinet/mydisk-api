using AutoFixture.Xunit2;
using FluentAssertions;
using Moq;
using MyDisk.Domain.Entities;
using MyDisk.Services.Disks.DTOs;
using MyDisk.Services.Disks.Queries;
using MyDisk.Services.Disks.Requests;
using MyDisk.Tests.Services;
using MyDisk.Tests.Utils;

namespace MyDisk.Services.Tests.Disks;

public class GetAllDisksQueryHandlerTestsShould
{
    [Theory]
    [AutoServiceData]
    public async Task FindData(
        GetAllDisksQueryHandler sut,
        List<Disk> disks
    )
    {
        // Arrange
        sut.DiskRepository.AsMock()
            .Setup(x => x.GetDisksAsync())
            .ReturnsAsync(disks);

        // Act
        var act = await sut.Handle(It.IsAny<GetAllDisksRequest>(), It.IsAny<CancellationToken>());

        // Assert
        sut.Mapper.AsMock()
            .Verify(x => x.Map<List<DiskEntity>>(disks), Times.Once);
        act.Should().NotBeNull();
    }

    [Theory]
    [AutoServiceData]
    public async Task NotFindData(
        GetAllDisksQueryHandler sut,
        [NoAutoProperties] List<Disk> disks
    )
    {
        // Arrange
        sut.DiskRepository.AsMock()
            .Setup(x => x.GetDisksAsync()).ReturnsAsync(disks);

        // Act
        var act = await sut.Handle(It.IsAny<GetAllDisksRequest>(), It.IsAny<CancellationToken>());

        // Assert
        sut.Mapper.AsMock()
            .Verify(x => x.Map<List<DiskEntity>>(disks), Times.Once);
        act.Should().BeEmpty();
    }
}