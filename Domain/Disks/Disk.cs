using MyDisks.Domain.Authors;

namespace MyDisks.Domain.Disks;

public class Disk : BaseEntity
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public DateTime? ReleaseDate { get; set; }
    public string? ImageUrl { get; set; }
    public Guid? AuthorId { get; set; }
    public virtual Author? Author { get; set; }
}