using MyDisks.Domain.Authors;
using MyDisks.Domain.Reviews;

namespace MyDisks.Domain.Disks;

public sealed class Disk : AggregateRoot<Guid>
{
    public Disk()
    {
        Reviews = new List<Review>();
    }

    public Name Name { get; set; }
    public DateTime? ReleaseDate { get; set; }
    public string? ImageUrl { get; set; }
    public Guid? AuthorId { get; set; }
    public Author? Author { get; set; }
    public List<Review> Reviews { get; set; }

    public void AddReview(Review review)
    {
        ValidateReviewIsNotDuplicated(review);
        ValidateReleaseDate();
        Reviews.Add(review);
    }

    private void ValidateReviewIsNotDuplicated(Review review)
    {
        if (Reviews.Contains(review))
            throw new InvalidOperationException("Cannot add an existing review");
    }

    private void ValidateReleaseDate()
    {
        if (ReleaseDate > DateTime.Now)
            throw new InvalidOperationException("Cannot add a review for a future disk release date");
    }
}