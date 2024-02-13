using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace MyDisks.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public partial class ReviewController : ControllerBase
{
    public ReviewController(IMediator mediator)
    {
        Mediator = mediator;
    }

    public IMediator Mediator { get; set; }
}