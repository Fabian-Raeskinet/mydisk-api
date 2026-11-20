using MyDisks.Contracts.Reviews;
using MyDisks.Services.Reviews;
using MyDisks.Tests.Api;
using MyDisks.Tests.Utils;

namespace MyDisks.Api.Tests.Controllers;

public class ReviewControllerFixture
{
    public class CreateReviewFixture
    {
        [Theory]
        [AutoApiData]
        public async Task Should_Use_Mediator(CreateReviewCommand command, [NoAutoProperties] ReviewController sut)
        {
            // Act
            DiskControllerFixture.SetAuthenticatedUser(sut);
            var act = await sut.CreateReview(command);

            // Assert
            sut.Mediator.AsMock()
                .Verify(x =>
                    x.Send(
                        It.Is<CreateReviewCommandRequest>(request =>
                            request.DiskId == command.DiskId
                            && request.Content == command.Content
                            && Math.Abs(request.Note - command.Note) < 2 &&
                            request.Title == command.Title), CancellationToken.None));
        }

        [Theory]
        [AutoApiData]
        public async Task Should_Return_AcceptedWithReviewId(CreateReviewCommand command, Guid reviewId,
            [NoAutoProperties] ReviewController sut)
        {
            // Arrange
            sut.Mediator.AsMock()
                .Setup(x => x.Send(It.IsAny<CreateReviewCommandRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(reviewId);

            // Act
            DiskControllerFixture.SetAuthenticatedUser(sut);
            var act = await sut.CreateReview(command);

            // Assert
            var accepted = act.Should().BeOfType<AcceptedAtActionResult>().Which;
            accepted.ActionName.Should().Be(nameof(ReviewController.GetReview));
            accepted.RouteValues.Should().Contain("reviewId", reviewId);
            accepted.Value.Should().Be(new CreateReviewResult(reviewId, ReviewStatus.Pending));
        }
    }

    public class ArchiveReviewFixture
    {
        [Theory]
        [AutoApiData]
        public async Task Should_Use_Mediator(ArchiveReviewCommand command, [NoAutoProperties] ReviewController sut)
        {
            // Act
            DiskControllerFixture.SetAuthenticatedUser(sut);
            var act = await sut.ArchiveReview(command);

            // Assert
            sut.Mediator.AsMock()
                .Verify(x => x
                    .Send(It.Is<ArchiveReviewCommandRequest>(request => request.ReviewId == command.ReviewId),
                        CancellationToken.None));
        }

        [Theory]
        [AutoApiData]
        public async Task Should_Return_NoContentResult(ArchiveReviewCommand command,
            [NoAutoProperties] ReviewController sut)
        {
            // Act
            DiskControllerFixture.SetAuthenticatedUser(sut);
            var act = await sut.ArchiveReview(command);

            // Assert
            act.Should().BeOfType<NoContentResult>();
        }
    }
}