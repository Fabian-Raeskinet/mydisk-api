using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MyDisks.Services.Messaging;
using RabbitMQ.Client;

namespace MyDisks.Messaging;

internal sealed class RabbitMqOutboxPublisher(
    IServiceScopeFactory scopeFactory,
    ConnectionFactory connectionFactory,
    IOptions<RabbitMqOptions> options,
    ILogger<RabbitMqOutboxPublisher> logger) : BackgroundService
{
    private const int BatchSize = 50;
    private static readonly TimeSpan PollingInterval = TimeSpan.FromSeconds(2);
    private static readonly TimeSpan ReconnectDelay = TimeSpan.FromSeconds(5);
    private static readonly TimeSpan PublishTimeout = TimeSpan.FromSeconds(15);

    private readonly string _exchange = options.Value.Exchange;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await PublishUntilDisconnectedAsync(stoppingToken);
            }
            catch (Exception exception) when (!stoppingToken.IsCancellationRequested)
            {
                logger.LogWarning(exception, "RabbitMQ outbox publisher stopped unexpectedly. Reconnecting in {Delay}.", ReconnectDelay);
            }

            await Task.Delay(ReconnectDelay, stoppingToken).ConfigureAwait(ConfigureAwaitOptions.SuppressThrowing);
        }
    }

    private async Task PublishUntilDisconnectedAsync(CancellationToken stoppingToken)
    {
        await using var connection = await connectionFactory.CreateConnectionAsync(stoppingToken);
        await using var channel = await connection.CreateChannelAsync(
            new CreateChannelOptions(publisherConfirmationsEnabled: true, publisherConfirmationTrackingEnabled: true),
            stoppingToken);

        while (channel.IsOpen && !stoppingToken.IsCancellationRequested)
        {
            await using var scope = scopeFactory.CreateAsyncScope();
            var published = await scope.ServiceProvider.GetRequiredService<IOutboxStore>()
                .PublishPendingAsync(BatchSize, (message, token) => PublishAsync(channel, message, token), stoppingToken);

            if (published < BatchSize)
                await Task.Delay(PollingInterval, stoppingToken).ConfigureAwait(ConfigureAwaitOptions.SuppressThrowing);
        }
    }

    private async Task PublishAsync(IChannel channel, OutboxMessage message, CancellationToken cancellationToken)
    {
        using var timeout = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        timeout.CancelAfter(PublishTimeout);

        var properties = new BasicProperties
        {
            ContentType = "application/json",
            Type = message.Type,
            MessageId = message.Id.ToString(),
            CorrelationId = message.CorrelationId,
            Timestamp = new AmqpTimestamp(message.OccurredOnUtc.ToUnixTimeSeconds()),
            Persistent = true
        };

        try
        {
            await channel.BasicPublishAsync(_exchange, message.Type, mandatory: true, properties,
                Encoding.UTF8.GetBytes(message.Payload), timeout.Token);
        }
        catch (Exception exception) when (!cancellationToken.IsCancellationRequested)
        {
            logger.LogWarning(exception, "Could not publish outbox message {MessageId} ({MessageType}).", message.Id, message.Type);
            throw;
        }
    }
}
