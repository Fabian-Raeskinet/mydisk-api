using Microsoft.Extensions.Logging;
using MyDisks.Domain.Reviews;
using MyDisks.Services.Reviews.Moderation;

namespace MyDisks.Services.Reviews;

public sealed class ModerateReviewCommandHandler(
    IReviewRepository reviewRepository,
    IReviewModerator moderator,
    ILogger<ModerateReviewCommandHandler> logger) : ICommandHandler<ModerateReviewCommandRequest>
{
    public async Task Handle(ModerateReviewCommandRequest request, CancellationToken cancellationToken)
    {
        var review = await reviewRepository.GetReviewByFilterAsync(review => review.Id == request.ReviewId);
        if (review is null)
        {
            logger.LogInformation("Review {ReviewId} no longer exists, moderation skipped.", request.ReviewId);
            return;
        }

        if (review.Status is not ReviewStatus.Pending)
        {
            logger.LogInformation("Review {ReviewId} is {Status}, moderation skipped.", review.Id, review.Status);
            return;
        }

        var result = await moderator.ModerateAsync(review.Title, review.Content, cancellationToken);
        if (result.Approved)
            review.Publish();
        else
            review.Reject();

        await reviewRepository.UpdateReviewAsync(review);
        logger.LogInformation("Review {ReviewId} moderated as {Status}. {Reason}", review.Id, review.Status, result.Reason);
    }
}
