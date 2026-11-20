namespace MyDisks.Services.Messaging;

public interface IOutboxStore
{
    Task<int> PublishPendingAsync(
        int batchSize,
        Func<OutboxMessage, CancellationToken, Task> publish,
        CancellationToken cancellationToken);
}
