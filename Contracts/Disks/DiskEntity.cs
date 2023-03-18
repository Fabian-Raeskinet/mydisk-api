namespace Contracts.Disks;

public class DiskEntity
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public DateTime? ReleaseDate { get; set; }
    public string? ImageUrl { get; set; }
    public AuthorResponse? Author { get; set; }
}