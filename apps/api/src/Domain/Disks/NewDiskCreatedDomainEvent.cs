namespace MyDisks.Domain.Disks;

public class NewDiskCreatedDomainEvent : DomainEvent
{
    public NewDiskCreatedDomainEvent(Disk disk)
    {
        Disk = disk;
    }

    public Disk Disk { get; set; }
}