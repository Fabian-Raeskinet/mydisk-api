using FluentAssertions;
using Moq;
using MyDisks.Domain.Exceptions;
using MyDisks.Domain.Reviews;
using MyDisks.Services.Reviews;
using MyDisks.Tests.Services;
using MyDisks.Tests.Utils;

namespace MyDisks.Services.Tests.Reviews;

public class ArchiveReviewCommandHandlerFixture
{
    public class HandleFixture
    {
        [Theory]
        [AutoServiceData]
        public async Task Should_GetReviewByFilterAsync
        (
            ArchiveReviewCommandRequest request,
            Review review,
            ArchiveReviewCommandHandler sut
        )
        {
            // Arrange
            sut.ReviewRepository.AsMock()
                .Setup(x => x.GetReviewByFilterAsync(r => r.Id == request.ReviewId))
                .ReturnsAsync(review);

            // Act
            await sut.Handle(request, CancellationToken.None);

            // Assert
            sut.ReviewRepository.AsMock()
                .Verify(x => x.GetReviewByFilterAsync(r => r.Id == request.ReviewId));
        }

        [Theory]
        [AutoServiceData]
        public async Task Should_Throws_ObjectNotFoundException
        (
            ArchiveReviewCommandRequest request,
            ArchiveReviewCommandHandler sut
        )
        {
            // Arrange
            sut.ReviewRepository.AsMock()
                .Setup(x => x.GetReviewByFilterAsync(r => r.Id == request.ReviewId))
                .ReturnsAsync((Review)null!);

            // Act
            var act = async () => await sut.Handle(request, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<ObjectNotFoundException>();
        }

        [Theory]
        [AutoServiceData]
        public async Task Should_UpdateReviewAsync
        (
            ArchiveReviewCommandRequest request,
            Review review,
            ArchiveReviewCommandHandler sut
        )
        {
            // Arrange
            sut.ReviewRepository.AsMock()
                .Setup(x => x.GetReviewByFilterAsync(r => r.Id == request.ReviewId))
                .ReturnsAsync(review);

            // Act
            await sut.Handle(request, CancellationToken.None);

            // Assert
            sut.ReviewRepository.AsMock()
                .Verify(x => x.UpdateReviewAsync(review));
        }
    }
}