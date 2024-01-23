using MediatR;
using Microsoft.Extensions.Logging;
using MyDisks.Domain.Disks;

namespace MyDisks.Services.Disks;

public class NewDiskDomainEventHandler : INotificationHandler<NewDiskDomainEvent>
{
    public NewDiskDomainEventHandler(ILogger<NewDiskDomainEventHandler> logger)
    {
        Logger = logger;
    }

    public ILogger<NewDiskDomainEventHandler> Logger { get; }
    
    public Task Handle(NewDiskDomainEvent notification, CancellationToken cancellationToken)
    {
        Logger.LogInformation("New Disk Domain Event");
        
        return Task.CompletedTask;
    }
}