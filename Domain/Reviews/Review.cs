namespace MyDisks.Domain.Reviews;

public class Review : AggregateRoot<Guid>
{
    public DateTimeOffset PublishedDate { get; init; }
    public ReviewStatus Status { get; set; }

    public string? Content
    {
        get => _content;
        set
        {
            EnsureNotArchived(nameof(Content));
            EnsureNotNullOrEmpty(value, nameof(Content));
            _content = value;
        }
    }

    public required string Title
    {
        get => _title!;
        set
        {
            EnsureNotNullOrEmpty(value, nameof(Title));
            EnsureNotArchived(nameof(Title));
            if (value.Length > TitleMaxLength)
                throw new ArgumentException("Title cannot exceed 30 characters");
            _title = value;
        }
    }

    public double Note
    {
        get => _note;
        init
        {
            EnsureNotArchived(nameof(Note));

            if (value is < MinNote or > MaxNote)
                throw new ArgumentOutOfRangeException($"Note must be between ${MinNote} and ${MaxNote}");
            _note = value;
        }
    }

    public void Archive()
    {
        Status = ReviewStatus.Archived;
    }

    private void EnsureNotArchived(string parameterName)
    {
        if (Status is ReviewStatus.Archived)
            throw new InvalidOperationException(
                $"Cannot modify {parameterName} of Review {Id} because archived");
    }

    private static void EnsureNotNullOrEmpty(string? value, string parameterName)
    {
        if (string.IsNullOrEmpty(value))
            throw new ArgumentNullException(parameterName);
    }

    private string? _title;
    private string? _content;
    private readonly double _note;

    private const int TitleMaxLength = 30;
    private const double MinNote = 0;
    private const double MaxNote = 5;
}