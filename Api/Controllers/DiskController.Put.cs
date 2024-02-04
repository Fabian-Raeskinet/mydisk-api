using Microsoft.AspNetCore.Mvc;
using MyDisks.Contracts.Disks;
using MyDisks.Services.Disks;

namespace MyDisks.Api.Controllers;

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

        await Mediator.Send(request);

        return NoContent();
    }
}