using MyDisks.Contracts.Reviews;

namespace MyDisks.Services.Reviews;

public sealed record GetReviewQueryRequest(Guid ReviewId, DiskOwner Owner) : IQuery<ReviewResult>;
