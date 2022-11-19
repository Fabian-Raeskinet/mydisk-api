using Microsoft.AspNetCore.Mvc;
using MyDisk.Services.Disks.Command;

namespace MyDisk.Api.Controllers
{
    public partial class DiskController
    {
        [HttpPost]
        [Route("create-disk")]
        [ProducesResponseType(typeof(Guid), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Guid), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateDisk([FromBody] CreateDiskCommand command)
        {
            var result = _mediator.Send(command);
            return result.Result != null ? Ok(result.Result) : BadRequest();
        }
    }
}
