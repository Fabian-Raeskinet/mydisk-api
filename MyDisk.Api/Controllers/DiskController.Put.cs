using Contracts.Disks;
using Microsoft.AspNetCore.Mvc;

namespace MyDisk.Api.Controllers;

public partial class DiskController
{
    [HttpPut]
    [Route("update-disk")]
    [ProducesResponseType(typeof(Guid), 200)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> UpdateDisk([FromBody] UpdateDiskCommand command)
    {
        return Ok(await Mediator.Send(command));
    }
}