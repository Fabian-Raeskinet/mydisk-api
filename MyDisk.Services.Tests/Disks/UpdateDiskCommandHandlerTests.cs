using System.Linq.Expressions;
using FluentAssertions;
using MediatorExtension.Disks;
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
        UpdateDiskCommandRequest request,
        Disk disk
    )
    {
        // Arrange
        request.ReleaseDate = DateTime.Now;
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
    }

    [Theory]
    [AutoServiceData]
    public async Task ShouldThrowEntityNotFoundException
    (
        UpdateDiskCommandHandler sut,
        UpdateDiskCommandRequest request
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

        await result.Should().ThrowAsync<ObjectNotFoundException>();
    }
}