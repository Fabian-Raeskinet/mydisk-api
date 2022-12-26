using System.Linq.Expressions;
using AutoFixture.Xunit2;
using AutoMapper;
using FluentAssertions;
using Moq;
using MyDisk.Domain.Entities;
using MyDisk.Domain.Interfaces.IRepositories;
using MyDisk.Domain.Tests;
using MyDisk.Services.Disks.DTOs;
using MyDisk.Services.Disks.Queries;
using MyDisk.Services.Disks.Requests;

namespace MyDisk.Services.Tests.Disks;

public class GetDiskByNameQueryHandlerTestsShould
{
    [Theory, AutoDomainData]
    public async Task FindDisk
    (
        [Frozen] Mock<IMapper> mapperMock,
        [Frozen] Mock<IDiskRepository> repositoryMock,
        GetDiskByNameQueryHandler sut,
        Disk disk
    )
    {
        repositoryMock.Setup(x => x.GetDiskByFilterAsync(It.IsAny<Expression<Func<Disk, bool>>>())).ReturnsAsync(disk);

        var result = await sut.Handle(It.IsAny<GetDiskByNameRequest>(), It.IsAny<CancellationToken>());
        
        mapperMock.Verify(x => x.Map<DiskResponse>(disk), Times.Once);
        result.Should().NotBeNull();
    }
    
    [Theory, AutoDomainData]
    public async Task NotFindDisk
    (
        [Frozen] Mock<IMapper> mapperMock,
        [Frozen] Mock<IDiskRepository> repositoryMock,
        GetDiskByNameQueryHandler sut
    )
    {
        repositoryMock.Setup(x => x.GetDiskByFilterAsync(It.IsAny<Expression<Func<Disk, bool>>>())).ReturnsAsync(() => null);

        var result = await sut.Handle(It.IsAny<GetDiskByNameRequest>(), It.IsAny<CancellationToken>());
        
        mapperMock.Verify(x => x.Map<DiskResponse>(It.IsAny<Disk>()), Times.Never);
        result.Should().BeNull();
    }
}