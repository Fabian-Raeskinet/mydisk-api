using Microsoft.EntityFrameworkCore;
using MyDisks.Services.Messaging;

namespace MyDisks.Data.Messaging;

internal sealed class OutboxStore(ApplicationDbContext context) : IOutboxStore
{
    public async Task<int> PublishPendingAsync(
        int batchSize,
        Func<OutboxMessage, CancellationToken, Task> publish,
        CancellationToken cancellationToken)
    {
        await using var transaction = await context.Database.BeginTransactionAsync(cancellationToken);

        var entries = await context.OutboxMessages
            .FromSql($"""
                SELECT * FROM "OutboxMessages"
                WHERE "ProcessedOnUtc" IS NULL
                  AND ("NextRetryOnUtc" IS NULL OR "NextRetryOnUtc" <= now())
                ORDER BY "OccurredOnUtc", "Id"
                LIMIT {batchSize}
                FOR UPDATE SKIP LOCKED
                """)
            .ToListAsync(cancellationToken);

        if (entries.Count == 0)
            return 0;

        var published = 0;
        foreach (var entry in entries)
        {
            try
            {
                await publish(new OutboxMessage(entry.Id, entry.Type, entry.Payload, entry.OccurredOnUtc, entry.CorrelationId), cancellationToken);
                entry.MarkProcessed(DateTimeOffset.UtcNow);
                published++;
            }
            catch (Exception exception) when (!cancellationToken.IsCancellationRequested)
            {
                entry.MarkFailed(exception.Message, DateTimeOffset.UtcNow);
                break;
            }
        }

        await context.SaveChangesAsync(CancellationToken.None);
        await transaction.CommitAsync(CancellationToken.None);
        return published;
    }
}
