using System.Linq.Expressions;
using AutoFixture.Xunit2;
using AutoMapper;
using FluentAssertions;
using Moq;
using MyDisk.Domain.Entities;
using MyDisk.Domain.Interfaces.IRepositories;
using MyDisk.Domain.Tests;
using MyDisk.Services.Common.Exceptions;
using MyDisk.Services.Disks.Commands;
using MyDisk.Services.Disks.DTOs;
using MyDisk.Services.Disks.Requests;

namespace MyDisk.Services.Tests.Disks;

public class UpdateDiskCommandHandlerTests
{
    [Theory, AutoDomainData]
    public async Task ShouldUpdateDisk
    (
        [Frozen] Mock<IDiskRepository> diskRepositoryMock,
        [Frozen] Mock<IMapper> mapper,
        Disk disk,
        UpdateDiskRequest request,
        UpdateDiskCommandHandler sut
    )
    {
        diskRepositoryMock.Setup(x => x.GetDiskByFilterAsync(It.IsAny<Expression<Func<Disk, bool>>>()))
            .ReturnsAsync(disk);

        var result = await sut.Handle(request, It.IsAny<CancellationToken>());

        diskRepositoryMock.Verify(x => x.GetDiskByFilterAsync(It.IsAny<Expression<Func<Disk, bool>>>()), Times.Once);
        diskRepositoryMock.Verify(x => x.UpdateDiskAsync(It.IsAny<Disk>()), Times.Once);
        mapper.Verify(x => x.Map<DiskResponse>(It.IsAny<Disk>()), Times.Once);
    }

    [Theory, AutoDomainData]
    public async Task ShouldThrowEntityNotFoundException
    (
        [Frozen] Mock<IDiskRepository> diskRepositoryMock,
        [Frozen] Mock<IMapper> mapper,
        UpdateDiskRequest request,
        UpdateDiskCommandHandler sut
    )
    {
        diskRepositoryMock.Setup(x => x.GetDiskByFilterAsync(It.IsAny<Expression<Func<Disk, bool>>>()))
            .ReturnsAsync(() => null);

        var result = async () => await sut.Handle(request, It.IsAny<CancellationToken>());

        diskRepositoryMock.Verify(x => x.UpdateDiskAsync(It.IsAny<Disk>()), Times.Never);
        mapper.Verify(x => x.Map<DiskResponse>(It.IsAny<Disk>()), Times.Never);
        await result.Should().ThrowAsync<EntityNotFoundException>();
    }
}