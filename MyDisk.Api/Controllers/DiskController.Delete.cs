using MediatR;
using Microsoft.AspNetCore.Mvc;
using MyDisk.Contracts.Disks;
using MyDisk.Services;

namespace MyDisk.Api.Controllers;

public partial class DiskController
{
    [HttpDelete]
    [Route("delete-by-id")]
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> DeleteDiskById([FromBody] Guid diskId)
    {
        await Mediator.Send(new Request<DeleteDiskCommand, Unit>
        {
            Value = new DeleteDiskCommand
            {
                Property = Contracts.Disks.DeleteDiskByProperty.Id,
                Value = diskId.ToString()
            }
        });
        return NoContent();
    }

    [HttpDelete]
    [Route("delete-by-name")]
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> DeleteDiskByName(string name)
    {
        await Mediator.Send(new Request<DeleteDiskCommand, Unit>
        {
            Value = new DeleteDiskCommand
            {
                Property = Contracts.Disks.DeleteDiskByProperty.Name,
                Value = name
            }
        });
        return NoContent();
    }

    [HttpDelete]
    [Route("delete-by-property")]
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> DeleteDiskByProperty([FromBody] DeleteDiskCommand request)
    {
        await Mediator.Send(new Request<DeleteDiskCommand, Unit>
        {
            Value = new DeleteDiskCommand
            {
                Property = request.Property,
                Value = request.Value
            }
        });
        return NoContent();
    }
}