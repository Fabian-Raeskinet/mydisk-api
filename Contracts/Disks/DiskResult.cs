using MyDisks.Contracts.Reviews;

namespace MyDisks.Contracts.Disks;

public class DiskResult
{
    public Guid Id { get; init; }
    public required string Name { get; init; }
    public DateTime? ReleaseDate { get; init; }
    public string? ImageUrl { get; init; }
    public AuthorResult? Author { get; init; }
    public List<ReviewResult>? Reviews { get; init; }
}