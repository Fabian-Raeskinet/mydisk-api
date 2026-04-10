namespace MyDisks.Contracts.Disks;

public record AuthorResult
{
    public Guid Id { get; init; }
    public required string Pseudonym { get; init; }
}