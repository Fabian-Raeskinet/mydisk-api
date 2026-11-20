namespace MyDisks.Services.Messaging;

public interface IOutboxWriter
{
    void Enqueue<TMessage>(string type, TMessage message);
}
