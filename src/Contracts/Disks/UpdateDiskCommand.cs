namespace MyDisks.Contracts.Disks;

public class UpdateDiskCommand
{
    public Guid Id { get; init; }
    public string? Name { get; init; }
    public DateTime? ReleaseDate { get; init; }
}