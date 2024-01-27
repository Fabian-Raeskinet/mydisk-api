using MyDisks.Domain.Authors;
using MyDisks.Domain.Reviews;

namespace MyDisks.Domain.Disks;

// public sealed class Disk : AggregateRoot<Guid>
public sealed class Disk
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public DateTime? ReleaseDate { get; set; }
    public string? ImageUrl { get; set; }
    public Guid? AuthorId { get; set; }
    public Author? Author { get; set; }
    public List<Review> Reviews { get; set; }
}