using AutoFixture.Xunit2;
using FluentAssertions;
using MediatR;
using Moq;
using MyDisk.Contracts.Disks;
using MyDisk.Domain.Entities;
using MyDisk.Domain.Exceptions;
using MyDisk.Services.Disks;
using MyDisk.Tests.Services;
using MyDisk.Tests.Utils;

namespace MyDisk.Services.Tests.Disks;

public class DeleteDiskCommandHandlerFixture
{
    [Theory]
    [AutoServiceData]
    public async Task Should_Throws_InvalidOperationException_Because_Null_Value
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
    public async Task Should_Get_Disk_By_Id
    (
        DeleteDiskCommandRequest request,
        DeleteDiskCommandHandler sut
    )
    {
        // Arrange
        request.Property = DeleteDiskByProperty.Id;

        // Act
        await sut.Handle(request, CancellationToken.None);

        // Assert
        sut.DiskRepository.AsMock()
            .Verify(_ => _.GetDiskByFilterAsync(disk => disk.Id == new Guid(request.Value!)));
    }

    [Theory]
    [AutoServiceData]
    public async Task Should_Get_Disk_By_Name
    (
        DeleteDiskCommandRequest request,
        DeleteDiskCommandHandler sut
    )
    {
        // Arrange
        request.Property = DeleteDiskByProperty.Name;

        // Act
        await sut.Handle(request, CancellationToken.None);

        // Assert
        sut.DiskRepository.AsMock()
            .Verify(_ => _.GetDiskByFilterAsync(disk => disk.Name == request.Value));
    }

    [Theory]
    [AutoServiceData]
    public async Task Should_Throws_InvalidOperationException_Because_Null_Property
    (
        DeleteDiskCommandRequest request,
        DeleteDiskCommandHandler sut
    )
    {
        // Arrange
        request.Property = (DeleteDiskByProperty)100;

        // Act
        var act = async () => await sut.Handle(request, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>();
    }

    [Theory]
    [AutoServiceData]
    public async Task Should_Throws_ObjectNotFoundException
    (
        DeleteDiskCommandRequest request,
        DeleteDiskCommandHandler sut
    )
    {
        // Arrange
        sut.DiskRepository.AsMock()
            .Setup(_ => _.GetDiskByFilterAsync(disk => disk.Id == new Guid(request.Value!)))
            .ReturnsAsync((Disk?)null);

        // Act
        var act = async () => await sut.Handle(request, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ObjectNotFoundException>();
    }

    [Theory]
    [AutoServiceData]
    public async Task Should_DeleteDiskAsync
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
            .Setup(_ => _.GetDiskByFilterAsync(d => d.Id == new Guid(request.Value!)))
            .ReturnsAsync(disk);

        // Act
        await sut.Handle(request, CancellationToken.None);

        // Assert
        sut.DiskRepository.AsMock()
            .Verify(x => x.DeleteDiskAsync(disk));
    }

    [Theory]
    [AutoServiceData]
    public async Task Should_Returns_Unit_Object
    (
        DeleteDiskCommandRequest request,
        Disk returnedDisk,
        DeleteDiskCommandHandler sut
    )
    {
        // Arrange
        request.Property = DeleteDiskByProperty.Id;
        request.Value = Guid.NewGuid().ToString();

        sut.DiskRepository.AsMock()
            .Setup(_ => _.GetDiskByFilterAsync(disk => disk.Id == new Guid(request.Value!)))
            .ReturnsAsync(returnedDisk);

        // Act
        var act = await sut.Handle(request, CancellationToken.None);

        // Assert
        act.Should().BeOfType<Unit>();
    }
}