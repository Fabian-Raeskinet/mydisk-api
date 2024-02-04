using FluentAssertions;
using Moq;
using MyDisks.Domain.Disks;
using MyDisks.Domain.Exceptions;
using MyDisks.Services.Disks;
using MyDisks.Tests.Services;
using MyDisks.Tests.Utils;

namespace MyDisks.Services.Tests.Disks;

public class UpdateDiskCommandHandlerFixture
{
    [Theory]
    [AutoServiceData]
    public async Task Should_Get_Disk_By_Id
    (
        UpdateDiskCommandRequest request,
        Disk disk,
        UpdateDiskCommandHandler sut
    )
    {
        // Arrange
        sut.DiskRepository.AsMock()
            .Setup(x => x.GetDiskByFilterAsync(disk => disk.Id == request.Id))
            .ReturnsAsync(disk);
        
        // Act
        await sut.Handle(request, CancellationToken.None);

        // Assert
        sut.DiskRepository.AsMock()
            .Verify(_ => _.GetDiskByFilterAsync(disk => disk.Id == request.Id));
    }

    [Theory]
    [AutoServiceData]
    public async Task Should_Throws_ObjectNotFoundException
    (
        UpdateDiskCommandRequest request,
        UpdateDiskCommandHandler sut
    )
    {
        // Arrange
        sut.DiskRepository.AsMock()
            .Setup(_ => _.GetDiskByFilterAsync(disk => disk.Id == request.Id))
            .ReturnsAsync((Disk?)null);

        // Act
        var act = async () => await sut.Handle(request, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ObjectNotFoundException>();
    }

    [Theory]
    [AutoServiceData]
    public async Task Should_UpdateDiskAsync
    (
        Disk disk,
        UpdateDiskCommandRequest request,
        UpdateDiskCommandHandler sut
    )
    {
        // Arrange
        sut.DiskRepository.AsMock()
            .Setup(_ => _.GetDiskByFilterAsync(d => d.Id == request.Id))
            .ReturnsAsync(disk);

        // Act
        await sut.Handle(request, CancellationToken.None);

        // Assert
        sut.DiskRepository.AsMock()
            .Verify(_ => _.UpdateDiskAsync(It.Is<Disk>(d =>
                d.Name == request.Name
                && d.ReleaseDate == request.ReleaseDate)));
    }

    [Theory]
    [AutoServiceData]
    public async Task Should_Not_Update_Name
    (
        Disk disk,
        UpdateDiskCommandRequest request,
        UpdateDiskCommandHandler sut
    )
    {
        // Arrange
        request.Name = null;

        sut.DiskRepository.AsMock()
            .Setup(_ => _.GetDiskByFilterAsync(d => d.Id == request.Id))
            .ReturnsAsync(disk);

        // Act
        await sut.Handle(request, CancellationToken.None);

        // Assert
        sut.DiskRepository.AsMock()
            .Verify(_ => _.UpdateDiskAsync(It.Is<Disk>(d =>
                d.Name == disk.Name)));
    }

    [Theory]
    [AutoServiceData]
    public async Task Should_Not_Update_ReleaseDate
    (
        Disk disk,
        UpdateDiskCommandRequest request,
        UpdateDiskCommandHandler sut
    )
    {
        // Arrange
        request.Name = null;

        sut.DiskRepository.AsMock()
            .Setup(_ => _.GetDiskByFilterAsync(d => d.Id == request.Id))
            .ReturnsAsync(disk);

        // Act
        await sut.Handle(request, CancellationToken.None);

        // Assert
        sut.DiskRepository.AsMock()
            .Verify(_ => _.UpdateDiskAsync(It.Is<Disk>(d =>
                d.ReleaseDate == disk.ReleaseDate)));
    }

    [Theory]
    [AutoServiceData]
    public async Task Should_Not_Update_Other_Properties
    (
        Disk disk,
        UpdateDiskCommandRequest request,
        UpdateDiskCommandHandler sut
    )
    {
        // Arrange
        sut.DiskRepository.AsMock()
            .Setup(_ => _.GetDiskByFilterAsync(d => d.Id == request.Id))
            .ReturnsAsync(disk);

        // Act
        await sut.Handle(request, CancellationToken.None);

        // Assert
        sut.DiskRepository.AsMock()
            .Verify(_ => _.UpdateDiskAsync(It.Is<Disk>(d =>
                d.Id == disk.Id
                && d.ImageUrl == disk.ImageUrl
                && d.AuthorId == disk.AuthorId)));
    }
}