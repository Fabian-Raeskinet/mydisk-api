using AutoFixture.Xunit2;
using FluentAssertions;
using Moq;
using MyDisk.Domain.Entities;
using MyDisk.Domain.Interfaces.IRepositories;
using MyDisk.Domain.Tests;
using MyDisk.Services.Disks.Commands;
using MyDisk.Services.Disks.Requests;

namespace MyDisk.Services.Tests.Disks;

public class CreateDiskCommandHandlerTestsShould
{
    [Theory, AutoDomainData]
    public async Task CreateDisk
    (
        [Frozen] Mock<IDiskRepository> repositoryMock,
        [Frozen] Disk disk,
        CreateDiskCommandHandler sut,
        CreateDiskRequest request
    )
    {
        request.ReleaseDate = "2022-12-20";
        repositoryMock.Setup(x => x.CreateDiskAsync(disk)).ReturnsAsync(disk.Id);

        var result = await sut.Handle(request, It.IsAny<CancellationToken>());

        repositoryMock.Verify(x => x.CreateDiskAsync(It.IsAny<Disk>()), Times.Once);
        result.Should().NotBeEmpty();
    }
}