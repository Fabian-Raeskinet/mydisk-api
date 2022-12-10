using Microsoft.AspNetCore.Mvc;
using MyDisk.Services.Disks.Requests;

namespace MyDisk.Api.Controllers
{
    public partial class DiskController
    {
        [HttpPost]
        [Route("create-disk")]
        [ProducesResponseType(typeof(Guid), 200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> CreateDisk([FromBody] CreateDiskRequest command)
        {
            return Ok(await _mediator.Send(command));
        }

        [HttpPost]
        [Route("attach-author")]
        [ProducesResponseType(typeof(Guid), 200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> AttachAuthor([FromBody] AttachAuthorRequest command)
        {
            var result = await _mediator.Send(command);
            return result != null ? Ok(result) : BadRequest();
        }
    }
}
