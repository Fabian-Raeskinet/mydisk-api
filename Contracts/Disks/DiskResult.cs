namespace MyDisks.Contracts.Disks;

public class DiskResult
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public DateTime? ReleaseDate { get; set; }
    public string? ImageUrl { get; set; }
    public AuthorResult? Author { get; set; }
}