using MediatorExtension.Disks;
using Microsoft.AspNetCore.Mvc;
using MyDisk.Contracts.Disks;

namespace MyDisk.Api.Controllers;

public partial class DiskController
{
    [HttpPost]
    [Route("create-disk")]
    [ProducesResponseType(typeof(Guid), 200)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> CreateDisk([FromBody] CreateDiskCommand command)
    {
        var request = new CreateDiskCommandRequest { Name = command.Name, ReleaseDate = command.ReleaseDate };
        return Ok(await Mediator.Send(request));
    }

    [HttpPost]
    [Route("attach-author")]
    [ProducesResponseType(typeof(Guid), 200)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> AttachAuthor([FromBody] AttachAuthorCommand command)
    {
        var request = new AttachAuthorCommandRequest { AuthorId = command.AuthorId, DiskId = command.DiskId };
        var result = await Mediator.Send(request);
        return Ok(result);
    }
}