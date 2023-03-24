using Microsoft.AspNetCore.Mvc;
using MyDisk.Contracts.Disks;
using MyDisk.Services;

namespace MyDisk.Api.Controllers;

public partial class DiskController
{
    [HttpPut]
    [Route("update-disk")]
    [ProducesResponseType(typeof(Guid), 200)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> UpdateDisk([FromBody] UpdateDiskCommand command)
    {
        return Ok(await Mediator.Send(new Request<UpdateDiskCommand, DiskResponse> { Value = command }));
    }
}