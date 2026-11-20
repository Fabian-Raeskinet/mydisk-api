using MyDisks.Domain.Disks;
using MyDisks.Domain.Exceptions;
using MyDisks.Domain.Reviews;

namespace MyDisks.Services.Reviews;

public class CreateReviewCommandHandler : ICommandHandler<CreateReviewCommandRequest, Guid>
{
    public CreateReviewCommandHandler(IDiskRepository diskRepository, IReviewRepository reviewRepository)
    {
        DiskRepository = diskRepository;
        ReviewRepository = reviewRepository;
    }

    public IDiskRepository DiskRepository { get; }
    public IReviewRepository ReviewRepository { get; }

    public async Task<Guid> Handle(CreateReviewCommandRequest request, CancellationToken cancellationToken)
    {
        var disk = await DiskRepository.GetDiskByFilterAsync(x =>
            x.Id == request.DiskId &&
            x.CreatedByIssuer == request.Owner.Issuer &&
            x.CreatedBySubject == request.Owner.Subject);

        if (disk is null)
            throw new ObjectNotFoundException();

        var review = new Review
        {
            Title = request.Title,
            Content = request.Content,
            Note = request.Note,
        };

        disk.AddReview(review);
        ReviewRepository.AddReview(review);
        review.AddDomainEvent(new NewReviewCreatedDomainEvent(review, disk.Id));

        await DiskRepository.UpdateDiskAsync(disk);
        return review.Id;
    }
}
