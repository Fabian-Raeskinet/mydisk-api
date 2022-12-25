﻿using Microsoft.AspNetCore.Mvc;
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
        await _mediator.Send(new DeleteDiskRequest { Property = DeleteDiskByPropertyEnum.Id, Value = diskId.ToString() });
        return NoContent();
    }
    
    [HttpDelete]
    [Route("delete-by-name")]
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> DeleteDiskByName(string name)
    {
        await _mediator.Send(new DeleteDiskRequest { Property = DeleteDiskByPropertyEnum.Name, Value = name });
        return NoContent();
    }
}