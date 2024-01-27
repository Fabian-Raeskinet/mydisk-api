using MediatR;

namespace MyDisks.Services;

public interface IDomainEventHandler<T> : INotificationHandler<T> where T : INotification
{
    Task Handle(T domainEvent);
}