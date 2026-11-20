namespace MyDisks.Services.Reviews;

public sealed record ModerateReviewCommandRequest(Guid ReviewId) : ICommand;
