using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using MyDisk.Services.Disks.Requests;

namespace MyDisk.Api.Controllers;

public partial class DiskController
{
    [HttpPut]
    [Route("update-disk")]
    [ProducesResponseType(typeof(Guid), 200)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> UpdateDisk([FromBody] UpdateDiskRequest command)
    {
        return Ok(await Mediator.Send(command));
    }
}