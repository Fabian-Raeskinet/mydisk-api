using MediatR;
using Microsoft.AspNetCore.Mvc;
using MyDisk.Services.Miscellaneous;

namespace MyDisk.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class MiscellaneousController : ControllerBase
{
    public MiscellaneousController(IMediator mediator)
    {
        Mediator = mediator;
    }

    public IMediator Mediator { get; }

    [HttpGet]
    [Route("disks")]
    [ProducesResponseType(204)]
    public async Task<IActionResult> RetryServiceAction()
    {
        await Mediator.Send(new RetryServiceRequest());
        return NoContent();
    }
}