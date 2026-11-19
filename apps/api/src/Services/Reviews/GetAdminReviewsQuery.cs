using MyDisks.Contracts.Reviews;
using MyDisks.Domain.Reviews;

namespace MyDisks.Services.Reviews;

public sealed record GetAdminReviewsQuery : IQuery<IReadOnlyList<AdminReviewResult>>;

public sealed class GetAdminReviewsQueryHandler(IReviewRepository reviewRepository)
    : IQueryHandler<GetAdminReviewsQuery, IReadOnlyList<AdminReviewResult>>
{
    public async Task<IReadOnlyList<AdminReviewResult>> Handle(GetAdminReviewsQuery request,
        CancellationToken cancellationToken)
    {
        var reviews = await reviewRepository.GetReviewsWithDisksAsync(cancellationToken);

        return reviews.Select(review => new AdminReviewResult(
            review.Id,
            review.DiskId,
            review.Disk!.Name.Value,
            review.Title,
            review.Content,
            review.Note,
            (Contracts.Reviews.ReviewStatus)review.Status)).ToList();
    }
}
