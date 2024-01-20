using MediatR;

namespace MyDisks.Domain.Disks;

public class NewDiskDomainEvent : DomainEvent, INotification
{
    public NewDiskDomainEvent(Disk disk)
    {
        Disk = disk;
    }

    public Disk Disk { get; set; }
}