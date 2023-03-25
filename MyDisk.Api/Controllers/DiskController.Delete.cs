using MediatorExtension.Disks;
using Microsoft.AspNetCore.Mvc;
using MyDisk.Contracts.Disks;

namespace MyDisk.Api.Controllers;

public partial class DiskController
{
    [HttpDelete]
    [Route("delete-by-id")]
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> DeleteDiskById([FromBody] Guid diskId)
    {
        await Mediator.Send(new DeleteDiskCommandRequest
        {
            Property = Contracts.Disks.DeleteDiskByProperty.Id,
            Value = diskId.ToString()
        });
        return NoContent();
    }

    [HttpDelete]
    [Route("delete-by-name")]
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> DeleteDiskByName(string name)
    {
        await Mediator.Send(new DeleteDiskCommandRequest
        {
            Property = Contracts.Disks.DeleteDiskByProperty.Name,
            Value = name
        });
        return NoContent();
    }

    [HttpDelete]
    [Route("delete-by-property")]
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> DeleteDiskByProperty([FromBody] DeleteDiskCommand command)
    {
        var request = new DeleteDiskCommandRequest { Property = command.Property, Value = command.Value };
        await Mediator.Send(request);
        return NoContent();
    }
}