namespace MyDisks.Domain.Disks;

public class NewDiskDomainEvent : DomainEvent
{
    public NewDiskDomainEvent(Disk disk)
    {
        Disk = disk;
    }

    public Disk Disk { get; set; }
}