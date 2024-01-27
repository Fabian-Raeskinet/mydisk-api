namespace MyDisks.Domain.Disks;

public class NewDiskDomainEvent : IDomainEvent
{
    public NewDiskDomainEvent(Disk disk)
    {
        Disk = disk;
    }

    public Disk Disk { get; set; }
}