using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace MyDisks.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public partial class DiskController : ControllerBase
{
    public DiskController(IMediator mediator)
    {
        Mediator = mediator;
    }

    public IMediator Mediator { get; }
}