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
        Disk disk,
        DeleteDiskCommandHandler sut
    )
    {
        // Arrange
        request.Property = DeleteDiskByProperty.Id;
        request.Value = Guid.NewGuid().ToString();

        sut.DiskRepository.AsMock()
            .Setup(x => x.GetDiskByFilterAsync(x => x.Id == new Guid(request.Value!)))
            .ReturnsAsync(disk);

        sut.DiskRepository.AsMock()
            .Setup(x => x.DeleteDiskAsync(disk))
            .ReturnsAsync(true);

        // Act
        await sut.Handle(request, CancellationToken.None);

        // Assert
        sut.DiskRepository.AsMock()
            .Verify(x => x.DeleteDiskAsync(disk), Times.Once);
    }

    [Theory]
    [AutoServiceData]
    public async Task ShouldDeleteByName
    (
        DeleteDiskCommandRequest request,
        Disk disk,
        DeleteDiskCommandHandler sut
    )
    {
        // Arrange
        request.Property = DeleteDiskByProperty.Name;

        sut.DiskRepository.AsMock()
            .Setup(x => x.GetDiskByFilterAsync(x => x.Name == request.Value))
            .ReturnsAsync(disk);

        sut.DiskRepository.AsMock()
            .Setup(x => x.DeleteDiskAsync(disk))
            .ReturnsAsync(true);

        // Act
        await sut.Handle(request, CancellationToken.None);

        // Assert
        sut.DiskRepository.AsMock()
            .Verify(x => x.DeleteDiskAsync(disk), Times.Once);
    }

    [Theory]
    [AutoServiceData]
    public async Task ShouldThrowInvalidOperationExceptionBecauseNullValue
    (
        [NoAutoProperties] DeleteDiskCommandRequest request,
        DeleteDiskCommandHandler sut
    )
    {
        // Arrange
        request.Value = null;

        // Act
        var act = async () => await sut.Handle(request, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>();
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
        request.Property = null;

        // Act
        var act = async () => await sut.Handle(request, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>();
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
            .Setup(x => x.GetDiskByFilterAsync(disk => disk.Id == new Guid(request.Value!)))
            .ReturnsAsync((Disk?)null);

        // Act
        var act = async () => await sut.Handle(request, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ObjectNotFoundException>();
    }
}