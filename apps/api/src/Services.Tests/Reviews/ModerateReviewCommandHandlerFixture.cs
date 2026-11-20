using System.Linq.Expressions;
using FluentAssertions;
using Moq;
using MyDisks.Domain.Reviews;
using MyDisks.Services.Reviews;
using MyDisks.Services.Reviews.Moderation;
using MyDisks.Tests.Services;
using MyDisks.Tests.Utils;

namespace MyDisks.Services.Tests.Reviews;

public class ModerateReviewCommandHandlerFixture
{
    public class HandleFixture
    {
        private static Review CreatePendingReview() => new() { Id = Guid.NewGuid(), Title = "Instant classic", Content = "Nothing to complain about" };

        private static Mock<IReviewRepository> RepositoryReturning(Review? review)
        {
            var repository = new Mock<IReviewRepository>();
            repository.Setup(x => x.GetReviewByFilterAsync(It.IsAny<Expression<Func<Review, bool>>>()))
                .ReturnsAsync(review);
            return repository;
        }

        private static ModerateReviewCommandHandler CreateSut(Mock<IReviewRepository> repository, Mock<IReviewModerator> moderator) =>
            new(repository.Object, moderator.Object,
                Microsoft.Extensions.Logging.Abstractions.NullLogger<ModerateReviewCommandHandler>.Instance);

        [Theory]
        [InlineData(true, ReviewStatus.Published)]
        [InlineData(false, ReviewStatus.Rejected)]
        public async Task Should_ApplyModerationDecision(bool approved, ReviewStatus expected)
        {
            var review = CreatePendingReview();
            var repository = RepositoryReturning(review);
            var moderator = new Mock<IReviewModerator>();
            moderator.Setup(x => x.ModerateAsync(review.Title, review.Content, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ReviewModerationResult(approved));

            await CreateSut(repository, moderator).Handle(new ModerateReviewCommandRequest(review.Id), CancellationToken.None);

            review.Status.Should().Be(expected);
            repository.Verify(x => x.UpdateReviewAsync(review), Times.Once);
        }

        [Fact]
        public async Task Should_Skip_AlreadyModeratedReview()
        {
            var review = CreatePendingReview();
            review.Publish();
            var repository = RepositoryReturning(review);
            var moderator = new Mock<IReviewModerator>();

            await CreateSut(repository, moderator).Handle(new ModerateReviewCommandRequest(review.Id), CancellationToken.None);

            review.Status.Should().Be(ReviewStatus.Published);
            moderator.VerifyNoOtherCalls();
            repository.Verify(x => x.UpdateReviewAsync(It.IsAny<Review>()), Times.Never);
        }

        [Fact]
        public async Task Should_Skip_MissingReview()
        {
            var repository = RepositoryReturning(null);
            var moderator = new Mock<IReviewModerator>();

            await CreateSut(repository, moderator).Handle(new ModerateReviewCommandRequest(Guid.NewGuid()), CancellationToken.None);

            moderator.VerifyNoOtherCalls();
            repository.Verify(x => x.UpdateReviewAsync(It.IsAny<Review>()), Times.Never);
        }
    }
}
