using FluentAssertions;
using Moq;
using MyDisk.Contracts.Disks;
using MyDisk.Domain.Entities;
using MyDisk.Domain.Exceptions;
using MyDisk.Services.Disks;
using MyDisk.Tests.Services;
using MyDisk.Tests.Utils;

namespace MyDisk.Services.Tests.Disks;

public class GetDiskByNameQueryHandlerFixture
{
    [Theory]
    [AutoServiceData]
    public async Task Should_Get_Disk_By_Name
    (
        GetDiskByNameQueryRequest request,
        GetDiskByNameQueryHandler sut
    )
    {
        // Act
        var act = await sut.Handle(request, CancellationToken.None);

        // Assert
        sut.DiskRepository.AsMock()
            .Verify(_ => _.GetDiskByFilterAsync(disk => disk.Name == request.Name));
    }

    [Theory]
    [AutoServiceData]
    public async Task Should_Throws_ObjectNotFoundException
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

    [Theory]
    [AutoServiceData]
    public async Task Should_Returns_Value
    (
        GetDiskByNameQueryRequest request,
        Disk disk,
        DiskResult diskResult,
        GetDiskByNameQueryHandler sut
    )
    {
        sut.DiskRepository.AsMock()
            .Setup(_ => _.GetDiskByFilterAsync(d => d.Name == request.Name))
            .ReturnsAsync(disk);

        sut.Mapper.AsMock()
            .Setup(_ => _.Map<DiskResult>(disk))
            .Returns(diskResult);

        // Act
        var act = await sut.Handle(request, CancellationToken.None);

        // Assert
        act.Should().NotBeNull();
    }
}