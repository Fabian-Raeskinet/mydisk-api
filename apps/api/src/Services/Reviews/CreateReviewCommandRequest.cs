using MyDisks.Contracts.Reviews;

namespace MyDisks.Services.Reviews;

public class CreateReviewCommandRequest : CreateReviewCommand, ICommand
{
    public required DiskOwner Owner { get; init; }
    
}