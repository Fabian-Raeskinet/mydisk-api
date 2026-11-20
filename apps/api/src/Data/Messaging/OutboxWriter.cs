using System.Diagnostics;
using System.Text.Json;
using MyDisks.Data.Configurations.Outbox;
using MyDisks.Services.Messaging;

namespace MyDisks.Data.Messaging;

internal sealed class OutboxWriter(ApplicationDbContext context) : IOutboxWriter
{
    private static readonly JsonSerializerOptions SerializerOptions = new(JsonSerializerDefaults.Web);

    public void Enqueue<TMessage>(string type, TMessage message)
    {
        context.OutboxMessages.Add(new OutboxEntry
        {
            Type = type,
            Payload = JsonSerializer.Serialize(message, SerializerOptions),
            OccurredOnUtc = DateTimeOffset.UtcNow,
            CorrelationId = CurrentCorrelationId()
        });
    }

    private static string CurrentCorrelationId()
    {
        return Activity.Current is { IdFormat: ActivityIdFormat.W3C } activity
            ? activity.TraceId.ToHexString()
            : Guid.NewGuid().ToString("N");
    }
}
