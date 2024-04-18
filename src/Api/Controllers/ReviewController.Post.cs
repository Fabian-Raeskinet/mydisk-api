using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using MyDisks.Contracts.Reviews;
using MyDisks.Services.Reviews;

namespace MyDisks.Api.Controllers;

public partial class ReviewController
{
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> CreateReview(CreateReviewCommand command)
    {
        var request = new CreateReviewCommandRequest()
        {
            Title = command.Title,
            Content = command.Content,
            DiskId = command.DiskId,
            Note = command.Note
        };
        await Mediator.Send(request);
        return NoContent();
    }

    [HttpPost("Archive")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> ArchiveReview(ArchiveReviewCommand command)
    {
        var request = new ArchiveReviewCommandRequest()
        {
            ReviewId = command.ReviewId
        };
        
        await Mediator.Send(request);
        return NoContent();
    }
}