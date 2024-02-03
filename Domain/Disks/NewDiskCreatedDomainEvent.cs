namespace MyDisks.Domain.Disks;

public class NewDiskCreatedDomainEvent : IDomainEvent
{
    public NewDiskCreatedDomainEvent(Disk disk)
    {
        Disk = disk;
    }

    public Disk Disk { get; set; }
}