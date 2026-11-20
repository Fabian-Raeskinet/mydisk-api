namespace MyDisks.Services.Reviews.Moderation;

public sealed record ReviewModerationResult(bool Approved, string? Reason = null)
{
    public static ReviewModerationResult Approve() => new(true);
    public static ReviewModerationResult Reject(string reason) => new(false, reason);
}
