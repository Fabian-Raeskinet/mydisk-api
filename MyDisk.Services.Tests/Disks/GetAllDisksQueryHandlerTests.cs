using AutoFixture.Xunit2;
using FluentAssertions;
using Moq;
using MyDisk.Contracts.Disks;
using MyDisk.Domain.Entities;
using MyDisk.Services.Disks;
using MyDisk.Tests.Services;
using MyDisk.Tests.Utils;

namespace MyDisk.Services.Tests.Disks;

public class GetAllDisksQueryHandlerTestsShould
{
    [Theory]
    [AutoServiceData]
    public async Task FindData(
        GetAllDisksQueryHandler sut,
        List<Disk> disks
    )
    {
        // Arrange
        sut.DiskRepository.AsMock()
            .Setup(x => x.GetDisksAsync())
            .ReturnsAsync(disks);

        // Act
        var act = await sut.Handle(It.IsAny<Request<GetAllDisksQuery, List<DiskResponse>>>(), It.IsAny<CancellationToken>());

        // Assert
        sut.Mapper.AsMock()
            .Verify(x => x.Map<List<DiskResponse>>(disks), Times.Once);
        act.Should().NotBeNull();
    }

    [Theory]
    [AutoServiceData]
    public async Task NotFindData(
        GetAllDisksQueryHandler sut,
        [NoAutoProperties] List<Disk> disks
    )
    {
        // Arrange
        sut.DiskRepository.AsMock()
            .Setup(x => x.GetDisksAsync()).ReturnsAsync(disks);

        // Act
        var act = await sut.Handle(It.IsAny<Request<GetAllDisksQuery, List<DiskResponse>>>(), It.IsAny<CancellationToken>());

        // Assert
        sut.Mapper.AsMock()
            .Verify(x => x.Map<List<DiskResponse>>(disks), Times.Once);
        act.Should().BeEmpty();
    }
}