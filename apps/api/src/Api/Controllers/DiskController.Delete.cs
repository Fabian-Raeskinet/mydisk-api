using Microsoft.AspNetCore.Mvc;
using MyDisks.Services.Disks;

namespace MyDisks.Api.Controllers;

public partial class DiskController
{
    [HttpDelete]
    [Route("delete-disk")]
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> DeleteDiskById([FromQuery] Guid diskId)
    {
        var owner = User.GetRequiredDiskOwner();

        await Mediator.Send(new DeleteDiskCommandRequest
        {
            DiskId = diskId,
            Owner = owner
        });
        return NoContent();
    }
}