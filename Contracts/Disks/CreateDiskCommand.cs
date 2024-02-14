namespace MyDisks.Contracts.Disks;

public class CreateDiskCommand
{
    public required string Name { get; init; }
    public DateTime ReleaseDate { get; init; }
}