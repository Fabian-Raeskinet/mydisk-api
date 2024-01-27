using MediatR;
using MyDisks.Domain;

namespace MyDisks.Services;

public class MediatRDomainEventDispatcher : IDomainEventDispatcher
{
    private readonly IMediator _mediator;

    public MediatRDomainEventDispatcher(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task Dispatch(IDomainEvent domainEvent)
    {
        await _mediator.Publish(domainEvent);
    }
}