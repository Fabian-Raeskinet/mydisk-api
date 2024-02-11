namespace MyDisks.Domain.Authors;

public class Author : AggregateRoot<Guid>
{
    public required Pseudonym Pseudonym { get; set; }
}