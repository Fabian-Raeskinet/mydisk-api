using MyDisks.Domain.Authors;
using MyDisks.Domain.Reviews;

namespace MyDisks.Domain.Disks;

public sealed class Disk : AggregateRoot<Guid>
{
    public Name Name { get; set; }
    public DateTime? ReleaseDate { get; set; }
    public string? ImageUrl { get; set; }
    public Guid? AuthorId { get; set; }
    public Author? Author { get; set; }
    public List<Review> Reviews { get; set; }

    public void AddReview(Review review)
    {
        if (ReleaseDate > DateTime.Now)
            throw new InvalidOperationException("cannot add a review for a futur disk release date");

        Reviews.Add(review);
    }
}