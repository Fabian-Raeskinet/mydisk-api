using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace MyDisk.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public partial class DiskController : ControllerBase
{
    private readonly IMediator _mediator;

    public DiskController(IMediator mediator)
    {
        _mediator = mediator;
    }
}