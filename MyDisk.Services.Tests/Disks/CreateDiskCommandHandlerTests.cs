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
        CreateDiskCommandRequest request,
        Disk disk
    )
    {
        // Arrange
        request.ReleaseDate = DateTime.Now;
        sut.DiskRepository.AsMock()
            .Setup(x => x.CreateDiskAsync(disk))
            .ReturnsAsync(disk.Id);

        // Act
        var act = await sut.Handle(request, CancellationToken.None);

        // Assert
        sut.DiskRepository.AsMock()
            .Verify(x => x.CreateDiskAsync(It.IsAny<Disk>()), Times.Once);
    }

    [Theory]
    [AutoServiceData]
    public async Task ThrowInvalidOperationException
    (
        CreateDiskCommandHandler sut,
        CreateDiskCommandRequest request
    )
    {
        // Arrange
        request.ReleaseDate = null;
        
        // Act
        var act = async () => await sut.Handle(request, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>();
    }
}