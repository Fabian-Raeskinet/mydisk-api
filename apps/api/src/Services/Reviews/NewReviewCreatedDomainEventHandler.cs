using MyDisks.Domain.Reviews;
using MyDisks.Services.Messaging;

namespace MyDisks.Services.Reviews;

public sealed class NewReviewCreatedDomainEventHandler(IOutboxWriter outbox)
    : IDomainEventHandler<NewReviewCreatedDomainEvent>
{
    public IOutboxWriter Outbox { get; } = outbox;

    public Task Handle(NewReviewCreatedDomainEvent notification, CancellationToken cancellationToken)
    {
        var review = notification.Review;
        Outbox.Enqueue(ReviewCreatedMessage.Type,
            new ReviewCreatedMessage(review.Id, notification.DiskId, review.PublishedDate));

        return Task.CompletedTask;
    }
}
