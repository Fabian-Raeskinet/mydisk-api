using MediatR;
using Microsoft.Extensions.Logging;
using MyDisks.Domain.Disks;

namespace MyDisks.Services.Disks;

public class NewDiskCreatedDomainEventHandler : IDomainEventHandler<NewDiskCreatedDomainEvent>
{
    public NewDiskCreatedDomainEventHandler(ILogger<NewDiskCreatedDomainEventHandler> logger)
    {
        Logger = logger;
    }

    public ILogger<NewDiskCreatedDomainEventHandler> Logger { get; }
    
    public Task Handle(NewDiskCreatedDomainEvent notification, CancellationToken cancellationToken)
    {
        Logger.LogInformation($"New Disk Domain Event {notification.Disk.Id}");
        
        return Task.CompletedTask;
    }
}