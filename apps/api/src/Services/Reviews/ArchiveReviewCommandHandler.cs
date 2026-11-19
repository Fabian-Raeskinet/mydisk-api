using MyDisks.Domain.Exceptions;
using MyDisks.Domain.Reviews;

namespace MyDisks.Services.Reviews;

public class ArchiveReviewCommandHandler : ICommandHandler<ArchiveReviewCommandRequest>
{
    public ArchiveReviewCommandHandler(IReviewRepository reviewRepository)
    {
        ReviewRepository = reviewRepository;
    }

    public IReviewRepository ReviewRepository { get; }

    public async Task Handle(ArchiveReviewCommandRequest request, CancellationToken cancellationToken)
    {
        var review = await ReviewRepository.GetReviewByFilterAsync(x =>
            x.Id == request.ReviewId && x.Disk != null &&
            x.Disk.CreatedByIssuer == request.Owner.Issuer &&
            x.Disk.CreatedBySubject == request.Owner.Subject);

        if (review is null)
            throw new ObjectNotFoundException();
        
        review.Archive();
        
        await ReviewRepository.UpdateReviewAsync(review);
    }
}