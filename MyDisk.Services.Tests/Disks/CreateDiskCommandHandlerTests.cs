using FluentAssertions;
using MediatorExtension;
using MediatR;
using Moq;
using MyDisk.Contracts.Disks;
using MyDisk.Domain.Entities;
using MyDisk.Services.Disks;
using MyDisk.Tests.Services;
using MyDisk.Tests.Utils;

namespace MyDisk.Services.Tests.Disks;

public class CreateDiskCommandHandlerTestsShould
{
    [Theory]
    [AutoServiceData]
    public async Task CreateDisk
    (
        CreateDiskCommandHandler sut,
        Request<CreateDiskCommand, Unit> request,
        Disk disk
    )
    {
        // Arrange
        request.Value.ReleaseDate = DateTime.Now;
        sut.DiskRepository.AsMock()
            .Setup(x => x.CreateDiskAsync(disk))
            .ReturnsAsync(disk.Id);

        // Act
        var act = await sut.Handle(request, It.IsAny<CancellationToken>());

        // Assert
        sut.DiskRepository.AsMock()
            .Verify(x => x.CreateDiskAsync(It.IsAny<Disk>()), Times.Once);
    }

    [Theory]
    [AutoServiceData]
    public async Task ThrowInvalidOperationException
    (
        CreateDiskCommandHandler sut,
        Request<CreateDiskCommand, Unit> request
    )
    {
        // Arrange
        request.Value.ReleaseDate = null;
        // Act
        var act = async () => await sut.Handle(request, It.IsAny<CancellationToken>());

        // Assert
        sut.DiskRepository.AsMock()
            .Verify(x => x.CreateDiskAsync(It.IsAny<Disk>()), Times.Never);
        await act.Should().ThrowAsync<InvalidOperationException>();
    }
}