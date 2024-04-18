namespace MyDisks.Contracts.Reviews;

public class CreateReviewCommand
{
    public Guid DiskId { get; set; }
    public required string Title { get; set; }
    public required string Content { get; set; }
    public required double Note { get; set; }
}