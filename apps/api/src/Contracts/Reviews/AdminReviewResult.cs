namespace MyDisks.Contracts.Reviews;

public sealed record AdminReviewResult(
    Guid Id,
    Guid DiskId,
    string DiskName,
    string Title,
    string? Content,
    double Note,
    ReviewStatus Status);
