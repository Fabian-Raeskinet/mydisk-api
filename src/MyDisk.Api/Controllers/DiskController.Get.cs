using Microsoft.AspNetCore.Mvc;
using MyDisk.Services.Disks.DTOs;
using MyDisk.Services.Disks.Requests;

namespace MyDisk.Api.Controllers
{
    public partial class DiskController
    {
        [HttpGet]
        [Route("disks")]
        [ProducesResponseType(typeof(List<DiskEntity>), 200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetAllDisks()
        {
            var query = new GetAllDisksRequest();
            var results = await _mediator.Send(query);
            return results.Any() ? Ok(results) : NotFound();
        }

        [HttpGet]
        [Route("disk-by-name")]
        [ProducesResponseType(typeof(DiskResponse), 200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetByName([FromQuery] string name)
        {
            var query = new GetDiskByNameRequest { Name = name };
            var results = await _mediator.Send(query);
            return results != null ? Ok(results) : NotFound();
        }
    }
}
