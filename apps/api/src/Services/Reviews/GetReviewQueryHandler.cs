using AutoMapper;
using MyDisks.Contracts.Reviews;
using MyDisks.Domain.Exceptions;
using MyDisks.Domain.Reviews;

namespace MyDisks.Services.Reviews;

public sealed class GetReviewQueryHandler(IReviewRepository reviewRepository, IMapper mapper)
    : IQueryHandler<GetReviewQueryRequest, ReviewResult>
{
    public async Task<ReviewResult> Handle(GetReviewQueryRequest request, CancellationToken cancellationToken)
    {
        var review = await reviewRepository.GetReviewByFilterAsync(review =>
            review.Id == request.ReviewId && review.Disk != null &&
            review.Disk.CreatedByIssuer == request.Owner.Issuer &&
            review.Disk.CreatedBySubject == request.Owner.Subject);

        if (review is null)
            throw new ObjectNotFoundException();

        return mapper.Map<ReviewResult>(review);
    }
}
