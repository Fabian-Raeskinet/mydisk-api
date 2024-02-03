using FluentAssertions;
using MediatR;
using Moq;
using MyDisks.Domain.Authors;
using MyDisks.Domain.Disks;
using MyDisks.Domain.Exceptions;
using MyDisks.Services.Disks;
using MyDisks.Tests.Services;
using MyDisks.Tests.Utils;

namespace MyDisks.Services.Tests.Disks;

public class AttachAuthorCommandHandlerFixture
{
    [Theory]
    [AutoServiceData]
    public async Task Should_Get_Author
    (
        AttachAuthorCommandRequest request,
        AttachAuthorCommandHandler sut
    )
    {
        // Act
        await sut.Handle(request, CancellationToken.None);

        // Assert
        sut.AuthorRepository.AsMock()
            .Verify(_ => _.GetAuthorByFilterAsync(author => author.Id == request.AuthorId));
    }

    [Theory]
    [AutoServiceData]
    public async Task Should_Get_Disk
    (
        AttachAuthorCommandRequest request,
        AttachAuthorCommandHandler sut
    )
    {
        // Act
        await sut.Handle(request, CancellationToken.None);

        // Assert
        sut.DiskRepository.AsMock()
            .Verify(_ => _.GetDiskByFilterAsync(disk => disk.Id == request.DiskId));
    }

    [Theory]
    [AutoServiceData]
    public async Task Should_Throws_ObjectNotFoundException_Because_Null_Author
    (
        AttachAuthorCommandRequest request,
        AttachAuthorCommandHandler sut
    )
    {
        // Arrange
        sut.AuthorRepository.AsMock()
            .Setup(x => x.GetAuthorByFilterAsync(author => author.Id == request.AuthorId))
            .ReturnsAsync((Author?)null);

        // Act
        var act = async () => await sut.Handle(request, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ObjectNotFoundException>();
    }

    [Theory]
    [AutoServiceData]
    public async Task Should_Throws_ObjectNotFoundException_Because_Null_Disk
    (
        AttachAuthorCommandRequest request,
        AttachAuthorCommandHandler sut
    )
    {
        // Arrange
        sut.DiskRepository.AsMock()
            .Setup(x => x.GetDiskByFilterAsync(disk => disk.Id == request.DiskId))
            .ReturnsAsync((Disk?)null);

        // Act
        var act = async () => await sut.Handle(request, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ObjectNotFoundException>();
    }

    [Theory]
    [AutoServiceData]
    public async Task Should_Throws_ObjectNotFoundException_With_Message
    (
        AttachAuthorCommandRequest request,
        AttachAuthorCommandHandler sut
    )
    {
        // Arrange
        sut.DiskRepository.AsMock()
            .Setup(x => x.GetDiskByFilterAsync(disk => disk.Id == request.DiskId))
            .ReturnsAsync((Disk?)null);

        // Act
        var act = async () => await sut.Handle(request, CancellationToken.None);

        // Assert
        await act.Should()
            .ThrowAsync<ObjectNotFoundException>()
            .WithMessage("no matches found");
    }
}