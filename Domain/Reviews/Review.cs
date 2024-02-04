using System.ComponentModel.DataAnnotations;

namespace MyDisks.Domain.Reviews;

public class Review : AggregateRoot<Guid>
{
    public Review(Guid id) : base(id)
    {
        Id = id;
    }

    public Guid Id { get; set; }
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
        init
        {
            if (string.IsNullOrEmpty(value))
                throw new ArgumentNullException();

            _title = value;
        }
    }

    public void Archive()
    {
        Status = ReviewStatus.Archived;
    }
}