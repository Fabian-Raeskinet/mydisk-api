using System.Linq.Expressions;
using AutoFixture.Xunit2;
using FluentAssertions;
using Moq;
using MyDisk.Domain.Entities;
using MyDisk.Domain.Interfaces.IRepositories;
using MyDisk.Domain.Tests;
using MyDisk.Services.Common.Enums;
using MyDisk.Services.Common.Exceptions;
using MyDisk.Services.Disks.Commands;
using MyDisk.Services.Disks.Requests;

namespace MyDisk.Services.Tests.Disks;

public class DeleteDiskCommandHandlerTestsShould
{
    [Theory, AutoDomainData]
    public async Task ShouldDeleteById
    (
        [Frozen] Mock<IDiskRepository> diskRepositoryMock,
        DeleteDiskRequest request,
        DeleteDiskCommandHandler sut,
        Disk disk
    )
    {
        request.Property = DeleteDiskByPropertyEnum.Id;
        diskRepositoryMock.Setup(x => x.GetDiskByFilterAsync(It.IsAny<Expression<Func<Disk, bool>>>()))
            .ReturnsAsync(disk);
        diskRepositoryMock.Setup(x => x.DeleteDiskAsync(It.IsAny<Disk>())).ReturnsAsync(true);

        var result = await sut.Handle(request, It.IsAny<CancellationToken>());

        diskRepositoryMock.Verify(x => x.GetDiskByFilterAsync(It.IsAny<Expression<Func<Disk, bool>>>()), Times.Once);
        diskRepositoryMock.Verify(x => x.DeleteDiskAsync(It.IsAny<Disk>()), Times.Once);
    }
    
    [Theory, AutoDomainData]
    public async Task ShouldDeleteByName
    (
        [Frozen] Mock<IDiskRepository> diskRepositoryMock,
        DeleteDiskRequest request,
        DeleteDiskCommandHandler sut,
        Disk disk
    )
    {
        request.Property = DeleteDiskByPropertyEnum.Name;
        diskRepositoryMock.Setup(x => x.GetDiskByFilterAsync(It.IsAny<Expression<Func<Disk, bool>>>()))
            .ReturnsAsync(disk);
        diskRepositoryMock.Setup(x => x.DeleteDiskAsync(It.IsAny<Disk>())).ReturnsAsync(true);

        var result = await sut.Handle(request, It.IsAny<CancellationToken>());

        diskRepositoryMock.Verify(x => x.GetDiskByFilterAsync(It.IsAny<Expression<Func<Disk, bool>>>()), Times.Once);
        diskRepositoryMock.Verify(x => x.DeleteDiskAsync(It.IsAny<Disk>()), Times.Once);
    }    
    
    [Theory, AutoDomainData]
    public async Task ShouldThrowInvalidOperationExceptionBecauseNullValue
    (
        [Frozen] Mock<IDiskRepository> diskRepositoryMock,
        [NoAutoProperties] DeleteDiskRequest request,
        DeleteDiskCommandHandler sut
    )
    {
        var result = async () => await sut.Handle(request, It.IsAny<CancellationToken>());

        await result.Should().ThrowAsync<InvalidOperationException>();
        diskRepositoryMock.Verify(x => x.GetDiskByFilterAsync(It.IsAny<Expression<Func<Disk, bool>>>()), Times.Never);
        diskRepositoryMock.Verify(x => x.DeleteDiskAsync(It.IsAny<Disk>()), Times.Never);
    }
    
    [Theory, AutoDomainData]
    public async Task ShouldThrowInvalidOperationExceptionBecauseIncorrectProperty
    (
        [Frozen] Mock<IDiskRepository> diskRepositoryMock,
        DeleteDiskRequest request,
        DeleteDiskCommandHandler sut
    )
    {
        request.Property = null;
        var result = async () => await sut.Handle(request, It.IsAny<CancellationToken>());

        await result.Should().ThrowAsync<InvalidOperationException>();
        diskRepositoryMock.Verify(x => x.GetDiskByFilterAsync(It.IsAny<Expression<Func<Disk, bool>>>()), Times.Never);
        diskRepositoryMock.Verify(x => x.DeleteDiskAsync(It.IsAny<Disk>()), Times.Never);
    }
    
    [Theory, AutoDomainData]
    public async Task ShouldThrowEntityNotFoundException
    (
        [Frozen] Mock<IDiskRepository> diskRepositoryMock,
        DeleteDiskRequest request,
        DeleteDiskCommandHandler sut
    )
    {
        diskRepositoryMock.Setup(x => x.GetDiskByFilterAsync(It.IsAny<Expression<Func<Disk, bool>>>()))
            .ReturnsAsync(() => null);
        
        var result = async () => await sut.Handle(request, It.IsAny<CancellationToken>());

        await result.Should().ThrowAsync<EntityNotFoundException>();
        diskRepositoryMock.Verify(x => x.GetDiskByFilterAsync(It.IsAny<Expression<Func<Disk, bool>>>()), Times.Once);
        diskRepositoryMock.Verify(x => x.DeleteDiskAsync(It.IsAny<Disk>()), Times.Never);
    }
}