using Microsoft.AspNetCore.Mvc;
using MyDisk.Contracts.Disks;
using MyDisk.Services.Disks;

namespace MyDisk.Api.Controllers;

public partial class DiskController
{
    [HttpPut]
    [Route("update-disk")]
    [ProducesResponseType(typeof(Guid), 200)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> UpdateDisk([FromBody] UpdateDiskCommand command)
    {
        var request = new UpdateDiskCommandRequest
            { Id = command.Id, Name = command.Name, ReleaseDate = command.ReleaseDate };
        return Ok(await Mediator.Send(request));
    }
}