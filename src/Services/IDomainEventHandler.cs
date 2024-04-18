using MediatR;
using MyDisks.Domain;

namespace MyDisks.Services;

public interface IDomainEventHandler<in T> : INotificationHandler<T> where T : IDomainEvent
{
    new Task Handle(T domainEvent, CancellationToken cancellationToken);
}