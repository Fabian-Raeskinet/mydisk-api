namespace MyDisks.Services.Messaging;

public sealed record OutboxMessage(
    Guid Id,
    string Type,
    string Payload,
    DateTimeOffset OccurredOnUtc,
    string CorrelationId);
