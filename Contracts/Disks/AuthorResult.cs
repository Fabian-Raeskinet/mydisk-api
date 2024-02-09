namespace MyDisks.Contracts.Disks;

public record AuthorResult
{
    public Guid Id { get; init; }
    public string Pseudonym { get; init; }
}