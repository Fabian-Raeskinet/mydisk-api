using Microsoft.AspNetCore.Mvc;
using MyDisk.Domain.Models;
using MyDisk.Services.Disks.DTOs;
using MyDisk.Services.Disks.Queries;

namespace MyDisk.Api.Controllers
{
    public partial class DiskController
    {
        [HttpGet]
        [Route("disks")]
        [ProducesResponseType(typeof(List<Disk>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(List<Disk>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAllDisks()
        {
            var query = new GetDisksQuery();
            var results = _mediator.Send(query);
            return results.Result.Count > 0 ? Ok(results.Result) : NotFound();
        }

        [HttpGet]
        [Route("disk-by-name")]
        [ProducesResponseType(typeof(DiskResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(DiskResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetByName([FromQuery] string name)
        {
            var query = new GetDiskByNameQuery { Name = name };
            var results = _mediator.Send(query);
            return results.Result != null ? Ok(results.Result) : NotFound();
        }
    }
}
