using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace MyDisk.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public partial class DiskController : ControllerBase
{
    public IMediator Mediator { get; }

    public DiskController(IMediator mediator)
    {
        Mediator = mediator;
    }
}