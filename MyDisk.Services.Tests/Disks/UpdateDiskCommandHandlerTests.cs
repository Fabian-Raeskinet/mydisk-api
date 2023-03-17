using System.Linq.Expressions;
using Contracts.Disks;
using FluentAssertions;
using Moq;
using MyDisk.Domain.Entities;
using MyDisk.Domain.Exceptions;
using MyDisk.Services.Disks;
using MyDisk.Tests.Services;
using MyDisk.Tests.Utils;

namespace MyDisk.Services.Tests.Disks;

public class UpdateDiskCommandHandlerTests
{
    [Theory]
    [AutoServiceData]
    public async Task ShouldUpdateDisk
    (
        UpdateDiskCommandHandler sut,
        UpdateDiskCommand request,
        Disk disk
    )
    {
        // Arrange
        sut.DiskRepository.AsMock()
            .Setup(x => x.GetDiskByFilterAsync(It.IsAny<Expression<Func<Disk, bool>>>()))
            .ReturnsAsync(disk);

        // Act
        var act = await sut.Handle(request, It.IsAny<CancellationToken>());

        // Assert
        sut.DiskRepository.AsMock()
            .Verify(x => x.GetDiskByFilterAsync(It.IsAny<Expression<Func<Disk, bool>>>()), Times.Once);
        sut.DiskRepository.AsMock()
            .Verify(x => x.UpdateDiskAsync(It.IsAny<Disk>()), Times.Once);
        sut.Mapper.AsMock()
            .Verify(x => x.Map<DiskResponse>(It.IsAny<Disk>()), Times.Once);
    }

    [Theory]
    [AutoServiceData]
    public async Task ShouldThrowEntityNotFoundException
    (
        UpdateDiskCommandHandler sut,
        UpdateDiskCommand request
    )
    {
        // Arrange
        sut.DiskRepository.AsMock()
            .Setup(x => x.GetDiskByFilterAsync(It.IsAny<Expression<Func<Disk, bool>>>()))
            .ReturnsAsync(() => null);

        // Act
        var result = async () => await sut.Handle(request, It.IsAny<CancellationToken>());

        // Assert
        sut.DiskRepository.AsMock()
            .Verify(x => x.UpdateDiskAsync(It.IsAny<Disk>()), Times.Never);
        sut.Mapper.AsMock()
            .Verify(x => x.Map<DiskResponse>(It.IsAny<Disk>()), Times.Never);
        await result.Should().ThrowAsync<ObjectNotFoundException>();
    }
}