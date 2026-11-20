using Microsoft.AspNetCore.Mvc;
using MyDisks.Contracts.Reviews;
using MyDisks.Services.Reviews;

namespace MyDisks.Api.Controllers;

public partial class ReviewController
{
    [HttpGet("{reviewId:guid}")]
    [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
    [ProducesResponseType(typeof(ReviewResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetReview(Guid reviewId, CancellationToken cancellationToken)
    {
        var owner = User.GetRequiredDiskOwner();

        var review = await Mediator.Send(new GetReviewQueryRequest(reviewId, owner), cancellationToken);
        return Ok(review);
    }
}
