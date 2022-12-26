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

public class GetAllDisksQueryHandlerTestsShould
{
    [Theory, AutoDomainData]
    public async Task FindData(
        [Frozen] Mock<IMapper> mapperMock,
        [Frozen] Mock<IDiskRepository> repositoryMock,
        GetAllDisksQueryHandler sut,
        List<Disk> disks
    )
    {
        repositoryMock.Setup(x => x.GetDisksAsync()).ReturnsAsync(disks);

        var result = await sut.Handle(It.IsAny<GetAllDisksRequest>(), It.IsAny<CancellationToken>());

        mapperMock.Verify(x => x.Map<List<DiskEntity>>(disks), Times.Once);
        result.Should().NotBeNull();
    }

    [Theory, AutoDomainData]
    public async Task NotFindData(
        [Frozen] Mock<IMapper> mapperMock,
        [Frozen] Mock<IDiskRepository> repositoryMock,
        GetAllDisksQueryHandler sut,
        [NoAutoProperties] List<Disk> disks
    )
    {
        repositoryMock.Setup(x => x.GetDisksAsync()).ReturnsAsync(disks);

        var result = await sut.Handle(It.IsAny<GetAllDisksRequest>(), It.IsAny<CancellationToken>());

        mapperMock.Verify(x => x.Map<List<DiskEntity>>(disks), Times.Once);
        result.Should().BeEmpty();
    }
}