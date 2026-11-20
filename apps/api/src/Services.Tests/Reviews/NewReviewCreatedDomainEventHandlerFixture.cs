using Moq;
using MyDisks.Domain.Reviews;
using MyDisks.Services.Reviews;
using MyDisks.Tests.Services;
using MyDisks.Tests.Utils;

namespace MyDisks.Services.Tests.Reviews;

public class NewReviewCreatedDomainEventHandlerFixture
{
    public class HandleFixture
    {
        [Theory]
        [AutoServiceData]
        public async Task Should_EnqueueReviewCreatedMessage
        (
            NewReviewCreatedDomainEvent domainEvent,
            NewReviewCreatedDomainEventHandler sut
        )
        {
            // Act
            await sut.Handle(domainEvent, CancellationToken.None);

            // Assert
            sut.Outbox.AsMock()
                .Verify(x => x.Enqueue(ReviewCreatedMessage.Type, new ReviewCreatedMessage(
                    domainEvent.Review.Id,
                    domainEvent.DiskId,
                    domainEvent.Review.PublishedDate)), Times.Once);
        }
    }
}
