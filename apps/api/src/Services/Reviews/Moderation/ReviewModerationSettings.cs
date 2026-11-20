namespace MyDisks.Services.Reviews.Moderation;

public sealed class ReviewModerationSettings
{
    public const string SectionName = "ReviewModeration";

    public TimeSpan SimulatedLatency { get; init; } = TimeSpan.Zero;
}
