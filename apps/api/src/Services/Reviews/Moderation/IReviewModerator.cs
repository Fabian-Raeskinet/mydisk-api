namespace MyDisks.Services.Reviews.Moderation;

public interface IReviewModerator
{
    Task<ReviewModerationResult> ModerateAsync(string title, string? content, CancellationToken cancellationToken);
}
