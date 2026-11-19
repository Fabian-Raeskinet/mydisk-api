using System.Linq.Expressions;
using FluentAssertions;
using Moq;
using MyDisks.Domain.Disks;
using MyDisks.Domain.Exceptions;
using MyDisks.Services.Reviews;
using MyDisks.Tests.Services;
using MyDisks.Tests.Utils;

namespace MyDisks.Services.Tests.Reviews;

public class CreateReviewCommandHandlerFixture
{
    public class HandleFixture
    {
        [Theory]
        [AutoServiceData]
        public async Task Should_GetDiskByFilterAsync
        (
            CreateReviewCommandRequest request,
            Disk disk,
            CreateReviewCommandHandler sut
        )
        {
            // Arrange
            sut.DiskRepository.AsMock()
                .Setup(x => x.GetDiskByFilterAsync(It.IsAny<Expression<Func<Disk, bool>>>()))
                .ReturnsAsync(disk);

            // Act
            await sut.Handle(request, CancellationToken.None);

            // Assert
            sut.DiskRepository.AsMock()
                .Verify(x => x.GetDiskByFilterAsync(It.IsAny<Expression<Func<Disk, bool>>>()));
        }

        [Theory]
        [AutoServiceData]
        public async Task Should_Throws_ObjectNotFoundException
        (
            CreateReviewCommandRequest request,
            CreateReviewCommandHandler sut
        )
        {
            // Arrange
            sut.DiskRepository.AsMock()
                .Setup(x => x.GetDiskByFilterAsync(It.IsAny<Expression<Func<Disk, bool>>>()))
                .ReturnsAsync((Disk)null!);

            // Act
            var act = async () => await sut.Handle(request, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<ObjectNotFoundException>();
        }

        [Theory]
        [AutoServiceData]
        public async Task Should_AddReview
        (
            CreateReviewCommandRequest request,
            Disk disk,
            CreateReviewCommandHandler sut
        )
        {
            // Arrange
            sut.DiskRepository.AsMock()
                .Setup(x => x.GetDiskByFilterAsync(It.IsAny<Expression<Func<Disk, bool>>>()))
                .ReturnsAsync(disk);

            // Act
            await sut.Handle(request, CancellationToken.None);

            // Assert
            disk.Reviews.Should().Contain(review =>
                review.Title == request.Title
                && review.Content == request.Content
                && Math.Abs(review.Note - request.Note) < 2);
        }

        [Theory]
        [AutoServiceData]
        public async Task Should_UpdateDiskAsync
        (
            CreateReviewCommandRequest request,
            Disk disk,
            CreateReviewCommandHandler sut
        )
        {
            // Arrange
            sut.DiskRepository.AsMock()
                .Setup(x => x.GetDiskByFilterAsync(It.IsAny<Expression<Func<Disk, bool>>>()))
                .ReturnsAsync(disk);

            // Act
            await sut.Handle(request, CancellationToken.None);

            // Assert
            sut.DiskRepository.AsMock()
                .Verify(x => x.UpdateDiskAsync(disk));
        }
    }
}