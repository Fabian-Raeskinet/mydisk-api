using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace MyDisks.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public partial class DiskController : ControllerBase
{
    public DiskController(IMediator mediator)
    {
        Mediator = mediator;
    }

    public IMediator Mediator { get; }
}
