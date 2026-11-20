namespace MyDisks.Data.Configurations.Outbox;

public sealed class OutboxEntry
{
    public const int TypeMaxLength = 255;
    public const int CorrelationIdMaxLength = 64;
    public const int ErrorMaxLength = 2000;

    private static readonly TimeSpan BaseRetryDelay = TimeSpan.FromSeconds(5);
    private static readonly TimeSpan MaxRetryDelay = TimeSpan.FromMinutes(5);

    public Guid Id { get; private set; }
    public required string Type { get; init; }
    public required string Payload { get; init; }
    public required DateTimeOffset OccurredOnUtc { get; init; }
    public required string CorrelationId { get; init; }
    public DateTimeOffset? ProcessedOnUtc { get; private set; }
    public int RetryCount { get; private set; }
    public DateTimeOffset? NextRetryOnUtc { get; private set; }
    public string? LastError { get; private set; }

    public void MarkProcessed(DateTimeOffset processedOnUtc)
    {
        ProcessedOnUtc = processedOnUtc;
        NextRetryOnUtc = null;
        LastError = null;
    }

    public void MarkFailed(string error, DateTimeOffset failedOnUtc)
    {
        RetryCount++;
        LastError = error.Length > ErrorMaxLength ? error[..ErrorMaxLength] : error;

        var delay = BaseRetryDelay * Math.Pow(2, Math.Min(RetryCount - 1, 10));
        NextRetryOnUtc = failedOnUtc + (delay < MaxRetryDelay ? delay : MaxRetryDelay);
    }
}
