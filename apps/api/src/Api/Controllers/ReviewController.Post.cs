using Microsoft.AspNetCore.Mvc;
using MyDisks.Contracts.Reviews;
using MyDisks.Services.Reviews;

namespace MyDisks.Api.Controllers;

public partial class ReviewController
{
    [HttpPost]
    [ProducesResponseType(typeof(CreateReviewResult), StatusCodes.Status202Accepted)]
    public async Task<IActionResult> CreateReview(CreateReviewCommand command)
    {
        var owner = User.GetRequiredDiskOwner();

        var request = new CreateReviewCommandRequest
        {
            Title = command.Title,
            Content = command.Content,
            DiskId = command.DiskId,
            Note = command.Note,
            Owner = owner
        };
        var reviewId = await Mediator.Send(request);
        return AcceptedAtAction(nameof(GetReview), new { reviewId },
            new CreateReviewResult(reviewId, ReviewStatus.Pending));
    }

    [HttpPost("Archive")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> ArchiveReview(ArchiveReviewCommand command)
    {
        var owner = User.GetRequiredDiskOwner();

        var request = new ArchiveReviewCommandRequest
        {
            ReviewId = command.ReviewId,
            Owner = owner
        };
        
        await Mediator.Send(request);
        return NoContent();
    }
}