using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace MyDisks.Messaging;

internal abstract class RabbitMqConsumer<TMessage>(
    IServiceScopeFactory scopeFactory,
    ConnectionFactory connectionFactory,
    ILogger logger) : BackgroundService where TMessage : class
{
    private const ushort PrefetchCount = 10;
    private static readonly TimeSpan ReconnectDelay = TimeSpan.FromSeconds(5);
    private static readonly TimeSpan RequeueDelay = TimeSpan.FromSeconds(1);
    private static readonly JsonSerializerOptions SerializerOptions = new(JsonSerializerDefaults.Web);

    protected abstract string QueueName { get; }

    protected abstract Task HandleAsync(TMessage message, IServiceProvider services, CancellationToken cancellationToken);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ConsumeUntilDisconnectedAsync(stoppingToken);
            }
            catch (Exception exception) when (!stoppingToken.IsCancellationRequested)
            {
                logger.LogWarning(exception, "RabbitMQ consumer of {Queue} stopped unexpectedly. Reconnecting in {Delay}.",
                    QueueName, ReconnectDelay);
            }

            await Task.Delay(ReconnectDelay, stoppingToken).ConfigureAwait(ConfigureAwaitOptions.SuppressThrowing);
        }
    }

    private async Task ConsumeUntilDisconnectedAsync(CancellationToken stoppingToken)
    {
        await using var connection = await connectionFactory.CreateConnectionAsync(stoppingToken);
        await using var channel = await connection.CreateChannelAsync(cancellationToken: stoppingToken);
        await channel.BasicQosAsync(prefetchSize: 0, PrefetchCount, global: false, stoppingToken);

        var closed = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        channel.ChannelShutdownAsync += (_, _) =>
        {
            closed.TrySetResult();
            return Task.CompletedTask;
        };

        var consumer = new AsyncEventingBasicConsumer(channel);
        consumer.ReceivedAsync += (_, delivery) => OnReceivedAsync(channel, delivery, stoppingToken);
        await channel.BasicConsumeAsync(QueueName, autoAck: false, consumer, stoppingToken);

        await closed.Task.WaitAsync(stoppingToken).ConfigureAwait(ConfigureAwaitOptions.SuppressThrowing);
    }

    private async Task OnReceivedAsync(IChannel channel, BasicDeliverEventArgs delivery, CancellationToken stoppingToken)
    {
        TMessage? message = null;
        try
        {
            message = JsonSerializer.Deserialize<TMessage>(delivery.Body.Span, SerializerOptions);
        }
        catch (JsonException exception)
        {
            logger.LogWarning(exception, "Message {MessageId} from {Queue} could not be deserialized.", delivery.BasicProperties.MessageId, QueueName);
        }

        if (message is null)
        {
            await channel.BasicRejectAsync(delivery.DeliveryTag, requeue: false, stoppingToken);
            return;
        }

        try
        {
            await using var scope = scopeFactory.CreateAsyncScope();
            await HandleAsync(message, scope.ServiceProvider, stoppingToken);
            await channel.BasicAckAsync(delivery.DeliveryTag, multiple: false, stoppingToken);
        }
        catch (Exception exception) when (!stoppingToken.IsCancellationRequested)
        {
            logger.LogWarning(exception, "Message {MessageId} from {Queue} failed and is requeued.", delivery.BasicProperties.MessageId, QueueName);
            await Task.Delay(RequeueDelay, stoppingToken).ConfigureAwait(ConfigureAwaitOptions.SuppressThrowing);
            await channel.BasicNackAsync(delivery.DeliveryTag, multiple: false, requeue: true, CancellationToken.None);
        }
    }
}
