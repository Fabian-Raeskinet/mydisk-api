using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyDisks.Contracts.Reviews;
using MyDisks.Services.Reviews;

namespace MyDisks.Api.Controllers;

[ApiController]
[Route("api/admin/reviews")]
[Authorize(Policy = "Admin")]
public sealed class AdminReviewsController(IMediator mediator, ILogger<AdminReviewsController> logger) : ControllerBase
{
    [HttpGet]
    [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<AdminReviewResult>>> GetReviews(CancellationToken cancellationToken)
    {
        var reviews = await mediator.Send(new GetAdminReviewsQuery(), cancellationToken);
        return Ok(reviews);
    }

    [HttpPost("{reviewId:guid}/archive")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ArchiveReview(Guid reviewId, CancellationToken cancellationToken)
    {
        var archived = await mediator.Send(new ArchiveReviewAsAdminCommand(reviewId), cancellationToken);
        if (archived)
        {
            logger.LogInformation("Administrator {ActorIssuer} {ActorSubject} archived review {ReviewId}.",
                User.FindFirst("iss")?.Value, User.FindFirst("sub")?.Value, reviewId);
        }

        return NoContent();
    }
}
