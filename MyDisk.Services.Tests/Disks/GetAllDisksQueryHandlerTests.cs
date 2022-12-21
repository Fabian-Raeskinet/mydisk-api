using AutoFixture.Xunit2;
using AutoMapper;
using FluentAssertions;
using Moq;
using MyDisk.Domain.Models;
using MyDisk.Domain.Tests;
using MyDisk.Infrastructure.Interfaces.IRepositories;
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
        GetAllDisksRequest request,
        List<Disk> disks
    )
    {
        repositoryMock.Setup(x => x.GetDisks()).Returns(disks);

        var result = await sut.Handle(It.IsAny<GetAllDisksRequest>(), It.IsAny<CancellationToken>());

        mapperMock.Verify(x => x.Map<List<DiskEntity>>(disks), Times.Once);
        result.Should().NotBeNull();
    }

    [Theory, AutoDomainData]
    public async Task NotFindData(
        [Frozen] Mock<IMapper> mapperMock,
        [Frozen] Mock<IDiskRepository> repositoryMock,
        GetAllDisksQueryHandler sut,
        GetAllDisksRequest request,
        [NoAutoProperties] List<Disk> disks
    )
    {
        repositoryMock.Setup(x => x.GetDisks()).Returns(disks);

        var result = await sut.Handle(It.IsAny<GetAllDisksRequest>(), It.IsAny<CancellationToken>());

        mapperMock.Verify(x => x.Map<List<DiskEntity>>(disks), Times.Once);
        result.Should().BeEmpty();
    }
}