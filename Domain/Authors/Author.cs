using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyDisks.Domain.Authors;

// public class Author : AggregateRoot<Guid>
public class Author
{
    public Guid Id { get; set; }

    public Pseudonym Pseudonym { get; set; }
}