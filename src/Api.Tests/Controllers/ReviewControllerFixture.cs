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
        public async Task Should_Return_NoContentResult(CreateReviewCommand command,
            [NoAutoProperties] ReviewController sut)
        {
            // Act
            var act = await sut.CreateReview(command);

            // Assert
            act.Should().BeOfType<NoContentResult>();
        }
    }

    public class ArchiveReviewFixture
    {
        [Theory]
        [AutoApiData]
        public async Task Should_Use_Mediator(ArchiveReviewCommand command, [NoAutoProperties] ReviewController sut)
        {
            // Act
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
            var act = await sut.ArchiveReview(command);

            // Assert
            act.Should().BeOfType<NoContentResult>();
        }
    }
}