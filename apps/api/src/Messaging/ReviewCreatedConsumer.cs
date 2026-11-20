using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MyDisks.Services.Reviews;
using RabbitMQ.Client;

namespace MyDisks.Messaging;

internal sealed class ReviewCreatedConsumer(
    IServiceScopeFactory scopeFactory,
    ConnectionFactory connectionFactory,
    ILogger<ReviewCreatedConsumer> logger)
    : RabbitMqConsumer<ReviewCreatedMessage>(scopeFactory, connectionFactory, logger)
{
    protected override string QueueName => "mydisk.review.created";

    protected override Task HandleAsync(ReviewCreatedMessage message, IServiceProvider services,
        CancellationToken cancellationToken)
    {
        return services.GetRequiredService<IMediator>().Send(new ModerateReviewCommandRequest(message.ReviewId), cancellationToken);
    }
}
