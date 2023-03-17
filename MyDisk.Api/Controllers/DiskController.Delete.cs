using Microsoft.AspNetCore.Mvc;
using MyDisk.Services.Common.Enums;
using MyDisk.Services.Disks.Requests;

namespace MyDisk.Api.Controllers;

public partial class DiskController
{
    [HttpDelete]
    [Route("delete-by-id")]
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> DeleteDiskById([FromBody] Guid diskId)
    {
        await Mediator.Send(new DeleteDiskRequest { Property = DeleteDiskByPropertyEnum.Id, Value = diskId.ToString() });
        return NoContent();
    }
    
    [HttpDelete]
    [Route("delete-by-name")]
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> DeleteDiskByName(string name)
    {
        await Mediator.Send(new DeleteDiskRequest { Property = DeleteDiskByPropertyEnum.Name, Value = name });
        return NoContent();
    }

    [HttpDelete]
    [Route("delete-by-property")]
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> DeleteDiskByName([FromBody] DeleteDiskRequest request)
    {
        await Mediator.Send(new DeleteDiskRequest { Property = request.Property, Value = request.Value });
        return NoContent();
    }
}