namespace MyDisks.Contracts.Disks;

public class CreateDiskCommand
{
    public string Name { get; init; }
    public DateTime ReleaseDate { get; init; }
}