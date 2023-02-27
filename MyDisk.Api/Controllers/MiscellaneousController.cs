using MediatR;
using Microsoft.AspNetCore.Mvc;
using MyDisk.Services.Miscellaneous;

namespace MyDisk.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class MiscellaneousController : ControllerBase
{
    private readonly IMediator _mediator;

    public MiscellaneousController(IMediator mediator)
    {
        _mediator = mediator;
    }
    
    [HttpGet]
    [Route("disks")]
    [ProducesResponseType(204)]
    public async Task<IActionResult> RetryServiceAction()
    {
         await _mediator.Send(new RetryServiceRequest());
        return NoContent();
    }
}