namespace MyDisks.Domain.Reviews;

public class NewReviewCreatedDomainEvent : DomainEvent
{
    public NewReviewCreatedDomainEvent(Review review, Guid diskId)
    {
        Review = review;
        DiskId = diskId;
    }

    public Review Review { get; }
    public Guid DiskId { get; }
}
