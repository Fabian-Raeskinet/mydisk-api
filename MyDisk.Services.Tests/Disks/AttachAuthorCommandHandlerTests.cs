using AutoFixture.Xunit2;
using AutoMapper;
using FluentAssertions;
using Moq;
using MyDisk.Domain.Models;
using MyDisk.Domain.Tests;
using MyDisk.Infrastructure.Interfaces.IRepositories;
using MyDisk.Services.Common.Exceptions;
using MyDisk.Services.Disks.Commands;
using MyDisk.Services.Disks.DTOs;
using MyDisk.Services.Disks.Requests;

namespace MyDisk.Services.Tests.Disks;

public class AttachAuthorCommandHandlerTestsShould
{
    [Theory, AutoDomainData]
    public async Task AttachAuthorToDisk
    (
        [Frozen] Mock<IAuthorRepository> authorRepositoryMock,
        [Frozen] Mock<IDiskRepository> diskRepositoryMock,
        [Frozen] Mock<IMapper> mapperMock,
        AttachAuthorRequest request,
        AttachAuthorCommandHandler sut,
        Author author,
        Disk disk
    )
    {
        authorRepositoryMock.Setup(x => x.GetAuthorByFilter(It.IsAny<Func<Author, bool>>())).Returns(author);
        diskRepositoryMock.Setup(x => x.GetDiskByFilter(It.IsAny<Func<Disk, bool>>())).Returns(disk);

        var result = await sut.Handle(request, It.IsAny<CancellationToken>());

        authorRepositoryMock.Verify(x => x.GetAuthorByFilter(It.IsAny<Func<Author, bool>>()), Times.Once);
        diskRepositoryMock.Verify(x => x.GetDiskByFilter(It.IsAny<Func<Disk, bool>>()), Times.Once);
        mapperMock.Verify(x => x.Map<DiskResponse>(It.IsAny<Disk>()), Times.Once);
        result.Should().NotBeNull();
    }

    [Theory, AutoDomainData]
    public async Task ThrowsEntityNotFoundException
    (
        [Frozen] Mock<IAuthorRepository> authorRepositoryMock,
        [Frozen] Mock<IDiskRepository> diskRepositoryMock,
        AttachAuthorRequest request,
        AttachAuthorCommandHandler sut
    )
    {
        authorRepositoryMock.Setup(x => x.GetAuthorByFilter(It.IsAny<Func<Author, bool>>())).Returns(() => null);
        diskRepositoryMock.Setup(x => x.GetDiskByFilter(It.IsAny<Func<Disk, bool>>())).Returns(() => null);

        var result = async () => await sut.Handle(request, It.IsAny<CancellationToken>());

        await result.Should().ThrowAsync<EntityNotFoundException>();
    }
}