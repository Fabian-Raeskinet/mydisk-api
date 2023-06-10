namespace MyDisk.Contracts.Disks;

public class AttachAuthorCommand
{
    // Test
    public Guid AuthorId { get; set; }
    public Guid DiskId { get; set; }
}