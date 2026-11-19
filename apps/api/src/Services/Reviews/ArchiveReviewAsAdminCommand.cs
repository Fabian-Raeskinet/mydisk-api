using MediatR;
using MyDisks.Domain.Exceptions;
using MyDisks.Domain.Reviews;

namespace MyDisks.Services.Reviews;

public sealed record ArchiveReviewAsAdminCommand(Guid ReviewId) : IRequest<bool>;

public sealed class ArchiveReviewAsAdminCommandHandler(IReviewRepository reviewRepository)
    : IRequestHandler<ArchiveReviewAsAdminCommand, bool>
{
    public async Task<bool> Handle(ArchiveReviewAsAdminCommand request, CancellationToken cancellationToken)
    {
        var review = await reviewRepository.GetReviewByFilterAsync(review => review.Id == request.ReviewId);
        if (review is null)
            throw new ObjectNotFoundException();

        if (review.Status == ReviewStatus.Archived)
            return false;

        review.Archive();
        await reviewRepository.UpdateReviewAsync(review);
        return true;
    }
}
