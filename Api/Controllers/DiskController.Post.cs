using Microsoft.AspNetCore.Mvc;
using MyDisk.Contracts.Disks;
using MyDisk.Services.Disks;

namespace MyDisk.Api.Controllers;

public partial class DiskController
{
    [HttpPost]
    [Route("create-disk")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> CreateDisk([FromBody] CreateDiskCommand command)
    {
        var request = new CreateDiskCommandRequest { Name = command.Name, ReleaseDate = command.ReleaseDate };
        await Mediator.Send(request);
        return NoContent();
    }

    [HttpPost]
    [Route("attach-author")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> AttachAuthor([FromBody] AttachAuthorCommand command)
    {
        var request = new AttachAuthorCommandRequest { AuthorId = command.AuthorId, DiskId = command.DiskId };
        await Mediator.Send(request);
        return NoContent();
    }
}