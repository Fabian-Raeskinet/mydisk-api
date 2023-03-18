using Contracts.Disks;
using Microsoft.AspNetCore.Mvc;

namespace MyDisk.Api.Controllers;

public partial class DiskController
{
    [HttpPost]
    [Route("create-disk")]
    [ProducesResponseType(typeof(Guid), 200)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> CreateDisk([FromBody] CreateDiskCommand command)
    {
        return Ok(await Mediator.Send(command));
    }

    [HttpPost]
    [Route("attach-author")]
    [ProducesResponseType(typeof(Guid), 200)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> AttachAuthor([FromBody] AttachAuthorCommand command)
    {
        var result = await Mediator.Send(command);
        return Ok(result);
    }
}