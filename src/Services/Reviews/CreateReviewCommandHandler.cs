using MyDisks.Domain.Disks;
using MyDisks.Domain.Exceptions;
using MyDisks.Domain.Reviews;

namespace MyDisks.Services.Reviews;

public class CreateReviewCommandHandler : ICommandHandler<CreateReviewCommandRequest>
{
    public CreateReviewCommandHandler(IDiskRepository diskRepository)
    {
        DiskRepository = diskRepository;
    }

    public IDiskRepository DiskRepository { get; }

    public async Task Handle(CreateReviewCommandRequest request, CancellationToken cancellationToken)
    {
        var disk = await DiskRepository.GetDiskByFilterAsync(x => x.Id == request.DiskId);

        if (disk is null)
            throw new ObjectNotFoundException();

        var review = new Review
        {
            Title = request.Title,
            Content = request.Content,
            Note = request.Note
        };
        
        disk.AddReview(review);
        
        await DiskRepository.UpdateDiskAsync(disk);
    }
}