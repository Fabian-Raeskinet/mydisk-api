namespace MyDisks.Contracts.Disks;

public class AttachAuthorCommand
{
    public Guid AuthorId { get; set; }
    public Guid DiskId { get; set; }
}