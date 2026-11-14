namespace MyDisks.Contracts.Disks;

public class AttachAuthorCommand
{
    public Guid AuthorId { get; init; }
    public Guid DiskId { get; init; }
}