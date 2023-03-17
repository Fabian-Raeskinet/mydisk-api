using System.Linq.Expressions;
using FluentAssertions;
using Moq;
using MyDisk.Domain.Entities;
using MyDisk.Services.Disks.DTOs;
using MyDisk.Services.Disks.Queries;
using MyDisk.Services.Disks.Requests;
using MyDisk.Tests.Services;
using MyDisk.Tests.Utils;

namespace MyDisk.Services.Tests.Disks;

public class GetDiskByNameQueryHandlerTestsShould
{
    [Theory]
    [AutoServiceData]
    public async Task FindDisk
    (
        GetDiskByNameQueryHandler sut,
        Disk disk
    )
    {
        // Arrange
        sut.DiskRepository.AsMock()
            .Setup(x => x.GetDiskByFilterAsync(It.IsAny<Expression<Func<Disk, bool>>>()))
            .ReturnsAsync(disk);

        // Act
        var act = await sut.Handle(It.IsAny<GetDiskByNameRequest>(), It.IsAny<CancellationToken>());

        // Assert
        sut.Mapper.AsMock()
            .Verify(x => x.Map<DiskResponse>(disk), Times.Once);
        act.Should().NotBeNull();
    }

    [Theory]
    [AutoServiceData]
    public async Task NotFindDisk
    (
        GetDiskByNameQueryHandler sut
    )
    {
        // Arrange
        sut.DiskRepository.AsMock()
            .Setup(x => x.GetDiskByFilterAsync(It.IsAny<Expression<Func<Disk, bool>>>()))
            .ReturnsAsync(() => null);

        // Act
        var act = await sut.Handle(It.IsAny<GetDiskByNameRequest>(), It.IsAny<CancellationToken>());

        // Assert
        sut.Mapper.AsMock()
            .Verify(x => x.Map<DiskResponse>(It.IsAny<Disk>()), Times.Never);
        act.Should().BeNull();
    }
}