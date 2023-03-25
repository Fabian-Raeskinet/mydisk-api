using MediatorExtension.Disks;
using Microsoft.AspNetCore.Mvc;
using MyDisk.Contracts.Disks;

namespace MyDisk.Api.Controllers;

public partial class DiskController
{
    [HttpGet]
    [Route("disks")]
    [ProducesResponseType(typeof(List<DiskResponse>), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetAllDisks()
    {
        var query = new GetAllDisksQueryRequest();
        var results = await Mediator.Send(query);
        return Ok(results);
    }

    [HttpGet]
    [Route("disk-by-name")]
    [ProducesResponseType(typeof(DiskResponse), 200)]
    [ProducesResponseType(typeof(List<string>), 400)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetByName([FromQuery] string? name)
    {
        var result = await Mediator.Send(new GetDiskByNameQueryRequest { Name = name });
        return Ok(result);
    }
}