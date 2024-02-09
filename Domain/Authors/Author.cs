namespace MyDisks.Domain.Authors;

public class Author : AggregateRoot<Guid>
{
    public Pseudonym Pseudonym { get; set; }
}