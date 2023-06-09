using System.Linq.Expressions;
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
        AttachAuthorCommandRequest request,
        AttachAuthorCommandHandler sut
    )
    {
        // Act
        var act = await sut.Handle(request, CancellationToken.None);

        // Assert
        sut.DiskRepository.AsMock()
            .Verify(x => x.GetDiskByFilterAsync(disk => disk.Id == request.DiskId), Times.Once);
    }

    [Theory]
    [AutoServiceData]
    public async Task ThrowsEntityNotFoundException
    (
        AttachAuthorCommandRequest request,
        AttachAuthorCommandHandler sut
    )
    {
        // Arrange
        sut.AuthorRepository.AsMock()
            .Setup(x => x.GetAuthorByFilterAsync(author => author.Id == request.AuthorId))
            .ReturnsAsync((Author?)null);

        sut.DiskRepository.AsMock()
            .Setup(x => x.GetDiskByFilterAsync(disk => disk.Id == request.DiskId))
            .ReturnsAsync((Disk?)null);

        // Act
        var act = async () => await sut.Handle(request, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ObjectNotFoundException>();
    }
}