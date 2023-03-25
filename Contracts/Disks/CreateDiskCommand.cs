namespace MyDisk.Contracts.Disks;

public class CreateDiskCommand
{
    public string? Name { get; set; }
    public DateTime? ReleaseDate { get; set; }
}