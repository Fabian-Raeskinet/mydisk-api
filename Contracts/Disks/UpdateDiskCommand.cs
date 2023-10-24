namespace MyDisks.Contracts.Disks;

public class UpdateDiskCommand
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public DateTime? ReleaseDate { get; set; }
}