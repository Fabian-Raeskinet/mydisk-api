using Contracts.Disks;
using FluentAssertions;
using Moq;
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
        CreateDiskCommand request,
        Disk disk
    )
    {
        // Arrange
        request.ReleaseDate = "2022-12-20";
        sut.DiskRepository.AsMock()
            .Setup(x => x.CreateDiskAsync(disk))
            .ReturnsAsync(disk.Id);

        // Act
        var act = await sut.Handle(request, It.IsAny<CancellationToken>());

        // Assert
        sut.DiskRepository.AsMock()
            .Verify(x => x.CreateDiskAsync(It.IsAny<Disk>()), Times.Once);
        act.Should().NotBeEmpty();
    }

    [Theory]
    [AutoServiceData]
    public async Task ThrowInvalidOperationException
    (
        CreateDiskCommandHandler sut,
        CreateDiskCommand request
    )
    {
        // Arrange
        request.ReleaseDate = null;
        // Act
        var act = async () => await sut.Handle(request, It.IsAny<CancellationToken>());

        // Assert
        sut.DiskRepository.AsMock()
            .Verify(x => x.CreateDiskAsync(It.IsAny<Disk>()), Times.Never);
        await act.Should().ThrowAsync<InvalidOperationException>();
    }
}