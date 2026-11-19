using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace MyDisks.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public partial class ReviewController : ControllerBase
{
    public ReviewController(IMediator mediator)
    {
        Mediator = mediator;
    }

    public IMediator Mediator { get; set; }
}
