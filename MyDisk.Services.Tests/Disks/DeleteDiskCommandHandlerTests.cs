using System.Linq.Expressions;
using AutoFixture.Xunit2;
using FluentAssertions;
using Moq;
using MyDisk.Contracts.Disks;
using MyDisk.Domain.Entities;
using MyDisk.Domain.Exceptions;
using MyDisk.Services.Disks;
using MyDisk.Tests.Services;
using MyDisk.Tests.Utils;

namespace MyDisk.Services.Tests.Disks;

public class DeleteDiskCommandHandlerTestsShould
{
    [Theory]
    [AutoServiceData]
    public async Task ShouldDeleteById
    (
        DeleteDiskCommandRequest request,
        DeleteDiskCommandHandler sut,
        Disk disk
    )
    {
        // Arrange
        request.Property = DeleteDiskByProperty.Id;

        sut.DiskRepository.AsMock()
            .Setup(x => x.GetDiskByFilterAsync(It.IsAny<Expression<Func<Disk, bool>>>()))
            .ReturnsAsync(disk);

        sut.DiskRepository.AsMock()
            .Setup(x => x.DeleteDiskAsync(It.IsAny<Disk>())).ReturnsAsync(true);

        // Act
        await sut.Handle(request, It.IsAny<CancellationToken>());

        // Assert
        sut.DiskRepository.AsMock()
            .Verify(x => x.GetDiskByFilterAsync(It.IsAny<Expression<Func<Disk, bool>>>()), Times.Once);

        sut.DiskRepository.AsMock()
            .Verify(x => x.DeleteDiskAsync(It.IsAny<Disk>()), Times.Once);
    }

    [Theory]
    [AutoServiceData]
    public async Task ShouldDeleteByName
    (
        DeleteDiskCommandRequest request,
        DeleteDiskCommandHandler sut,
        Disk disk
    )
    {
        // Arrange
        request.Property = DeleteDiskByProperty.Name;

        sut.DiskRepository.AsMock()
            .Setup(x => x.GetDiskByFilterAsync(It.IsAny<Expression<Func<Disk, bool>>>()))
            .ReturnsAsync(disk);

        sut.DiskRepository.AsMock()
            .Setup(x => x.DeleteDiskAsync(It.IsAny<Disk>())).ReturnsAsync(true);

        // Act
        await sut.Handle(request, It.IsAny<CancellationToken>());

        // Assert
        sut.DiskRepository.AsMock()
            .Verify(x => x.GetDiskByFilterAsync(It.IsAny<Expression<Func<Disk, bool>>>()), Times.Once);

        sut.DiskRepository.AsMock()
            .Verify(x => x.DeleteDiskAsync(It.IsAny<Disk>()), Times.Once);
    }

    [Theory]
    [AutoServiceData]
    public async Task ShouldThrowInvalidOperationExceptionBecauseNullValue
    (
        [NoAutoProperties] DeleteDiskCommandRequest request,
        DeleteDiskCommandHandler sut
    )
    {
        // Act
        var act = async () => await sut.Handle(request, It.IsAny<CancellationToken>());

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>();
        sut.DiskRepository.AsMock()
            .Verify(x => x.GetDiskByFilterAsync(It.IsAny<Expression<Func<Disk, bool>>>()), Times.Never);
        sut.DiskRepository.AsMock()
            .Verify(x => x.DeleteDiskAsync(It.IsAny<Disk>()), Times.Never);
    }

    [Theory]
    [AutoServiceData]
    public async Task ShouldThrowInvalidOperationExceptionBecauseIncorrectProperty
    (
        [NoAutoProperties] DeleteDiskCommandRequest request,
        DeleteDiskCommandHandler sut
    )
    {
        // Arrange
        var act = async () => await sut.Handle(request, It.IsAny<CancellationToken>());

        // Act
        await act.Should().ThrowAsync<InvalidOperationException>();

        // Assert
        sut.DiskRepository.AsMock()
            .Verify(x => x.GetDiskByFilterAsync(It.IsAny<Expression<Func<Disk, bool>>>()), Times.Never);
        sut.DiskRepository.AsMock()
            .Verify(x => x.DeleteDiskAsync(It.IsAny<Disk>()), Times.Never);
    }

    [Theory]
    [AutoServiceData]
    public async Task ShouldThrowEntityNotFoundException
    (
        DeleteDiskCommandRequest request,
        DeleteDiskCommandHandler sut
    )
    {
        // Arrange
        sut.DiskRepository.AsMock()
            .Setup(x => x.GetDiskByFilterAsync(It.IsAny<Expression<Func<Disk, bool>>>()))
            .ReturnsAsync(() => null);

        // Act
        var act = async () => await sut.Handle(request, It.IsAny<CancellationToken>());

        // Assert
        await act.Should().ThrowAsync<ObjectNotFoundException>();

        sut.DiskRepository.AsMock()
            .Verify(x => x.GetDiskByFilterAsync(It.IsAny<Expression<Func<Disk, bool>>>()), Times.Once);

        sut.DiskRepository.AsMock()
            .Verify(x => x.DeleteDiskAsync(It.IsAny<Disk>()), Times.Never);
    }

    [Theory]
    [AutoServiceData]
    public async Task ShouldThrow
    (
        DeleteDiskCommandRequest request,
        DeleteDiskCommandHandler sut
    )
    {
        // Arrange
        request.Property = null;
        sut.DiskRepository.AsMock()
            .Setup(x => x.GetDiskByFilterAsync(It.IsAny<Expression<Func<Disk, bool>>>()))
            .ReturnsAsync(() => null);

        // Act
        var act = async () => await sut.Handle(request, It.IsAny<CancellationToken>());

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>();

        sut.DiskRepository.AsMock()
            .Verify(x => x.GetDiskByFilterAsync(It.IsAny<Expression<Func<Disk, bool>>>()), Times.Never);

        sut.DiskRepository.AsMock()
            .Verify(x => x.DeleteDiskAsync(It.IsAny<Disk>()), Times.Never);
    }
}