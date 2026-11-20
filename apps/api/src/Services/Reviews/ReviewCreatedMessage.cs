namespace MyDisks.Services.Reviews;

public sealed record ReviewCreatedMessage(Guid ReviewId, Guid DiskId, DateTimeOffset PublishedDate)
{
    public const string Type = "review.created";
}
