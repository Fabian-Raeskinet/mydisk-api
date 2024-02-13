namespace MyDisks.Contracts.Reviews;

public class ReviewResult
{
    public DateTimeOffset PublishedDate { get; set; }
    public ReviewStatus Status { get; set; }
    public string? Content { get; set; }
    public string? Title { get; set; }
    public double Note { get; set; }
    
}