using System.Linq.Expressions;
using Contracts.Disks;
using FluentAssertions;
using Moq;
using MyDisk.Domain.Entities;
using MyDisk.Domain.Exceptions;
using MyDisk.Services.Disks;
using MyDisk.Tests.Services;
using MyDisk.Tests.Utils;

namespace MyDisk.Services.Tests.Disks;

public class AttachAuthorCommandHandlerTestsShould
{
    [Theory]
    [AutoServiceData]
    public async Task AttachAuthorToDisk
    (
        AttachAuthorCommand request,
        AttachAuthorCommandHandler sut,
        DiskResponse diskResponse
    )
    {
        // Arrange
        sut.Mapper.AsMock()
            .Setup(x => x.Map<DiskResponse>(It.IsAny<Disk>()))
            .Returns(diskResponse);

        // Act
        var act = await sut.Handle(request, It.IsAny<CancellationToken>());

        // Assert
        sut.Mapper.AsMock()
            .Verify(x => x.Map<DiskResponse>(It.IsAny<Disk>()), Times.Once);
        act.Should().BeEquivalentTo(diskResponse);
    }

    [Theory]
    [AutoServiceData]
    public async Task ThrowsEntityNotFoundException
    (
        AttachAuthorCommand request,
        AttachAuthorCommandHandler sut
    )
    {
        // Arrange
        sut.AuthorRepository.AsMock()
            .Setup(x => x.GetAuthorByFilterAsync(It.IsAny<Expression<Func<Author, bool>>>()))
            .ReturnsAsync(() => null);

        sut.DiskRepository.AsMock()
            .Setup(x => x.GetDiskByFilterAsync(It.IsAny<Expression<Func<Disk, bool>>>()))
            .ReturnsAsync(() => null);

        // Act
        var act = async () => await sut.Handle(request, It.IsAny<CancellationToken>());

        // Assert
        await act.Should().ThrowAsync<ObjectNotFoundException>();
    }
}