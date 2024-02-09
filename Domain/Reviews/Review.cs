using System.ComponentModel.DataAnnotations;

namespace MyDisks.Domain.Reviews;

public class Review : AggregateRoot<Guid>
{
    private string? _title { get; set; }
    private string? _content;
    public double Note { get; set; }
    public DateTimeOffset PublishedDate { get; init; }
    public ReviewStatus Status { get; set; }

    public string? Content
    {
        get => _content;
        set
        {
            if (Status is ReviewStatus.Archived)
                throw new InvalidOperationException($"Cannot Edit content of Review {Id} because archived");

            if (string.IsNullOrEmpty(value))
                throw new ArgumentNullException();

            _content = value;
        }
    }

    public string? Title
    {
        get => _title;
        set
        {
            if (string.IsNullOrEmpty(value))
                throw new ArgumentNullException($"Title cannot be empty");

            if (Status is ReviewStatus.Archived)
                throw new InvalidOperationException(
                    $"Cannot set Title of Review {Id} because archived");

            if (value.Length > 30)
                throw new ArgumentException("Title cannot exceed 30 characters");

            _title = value;
        }
    }

    public void Archive()
    {
        Status = ReviewStatus.Archived;
    }
}