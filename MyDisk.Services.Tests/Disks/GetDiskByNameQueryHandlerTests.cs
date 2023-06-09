using System.Linq.Expressions;
using FluentAssertions;
using Moq;
using MyDisk.Contracts.Disks;
using MyDisk.Domain.Entities;
using MyDisk.Domain.Exceptions;
using MyDisk.Services.Disks;
using MyDisk.Tests.Services;
using MyDisk.Tests.Utils;

namespace MyDisk.Services.Tests.Disks;

public class GetDiskByNameQueryHandlerTestsShould
{
    [Theory]
    [AutoServiceData]
    public async Task FindDisk
    (
        GetDiskByNameQueryRequest request,
        Disk disk,
        GetDiskByNameQueryHandler sut
    )
    {
        // Arrange
        sut.DiskRepository.AsMock()
            .Setup(x => x.GetDiskByFilterAsync(disk => disk.Name == request.Name))
            .ReturnsAsync(disk);

        // Act
        var act = await sut.Handle(It.IsAny<GetDiskByNameQueryRequest>(),
            CancellationToken.None);

        // Assert
        sut.Mapper.AsMock()
            .Verify(x => x.Map<DiskResult>(disk), Times.Once);
        act.Should().NotBeNull();
    }

    [Theory]
    [AutoServiceData]
    public async Task NotFindDisk
    (
        GetDiskByNameQueryRequest request,
        GetDiskByNameQueryHandler sut
    )
    {
        // Arrange
        sut.DiskRepository.AsMock()
            .Setup(x => x.GetDiskByFilterAsync(disk => disk.Name == request.Name))
            .ReturnsAsync(() => null);

        // Act
        var act = async () =>
            await sut.Handle(request, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ObjectNotFoundException>();
    }
}