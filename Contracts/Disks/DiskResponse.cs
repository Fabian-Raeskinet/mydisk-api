namespace Contracts.Disks;

public class DiskResponse
{
    public string? Name { get; set; }
    public AuthorResponse? Author { get; set; }
    public DateTime? ReleaseDate { get; set; }
}