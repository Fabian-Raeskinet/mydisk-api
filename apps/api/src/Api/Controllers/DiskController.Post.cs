using Microsoft.AspNetCore.Mvc;
using MyDisks.Contracts.Disks;
using MyDisks.Services.Disks;

namespace MyDisks.Api.Controllers;

public partial class DiskController
{
    [HttpPost]
    [Route("create-disk")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(400)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> CreateDisk([FromBody] CreateDiskCommand command)
    {
        var owner = User.GetRequiredDiskOwner();

        var request = new CreateDiskCommandRequest
        {
            Name = command.Name,
            ReleaseDate = command.ReleaseDate,
            CreatedByIssuer = owner.Issuer,
            CreatedBySubject = owner.Subject
        };
        await Mediator.Send(request);
        return NoContent();
    }

    [HttpPost]
    [Route("attach-author")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> AttachAuthor([FromBody] AttachAuthorCommand command)
    {
        var owner = User.GetRequiredDiskOwner();

        var request = new AttachAuthorCommandRequest
        {
            AuthorId = command.AuthorId,
            DiskId = command.DiskId,
            Owner = owner
        };
        await Mediator.Send(request);
        return NoContent();
    }
}
