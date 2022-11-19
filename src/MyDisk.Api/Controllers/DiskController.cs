using MediatR;
using Microsoft.AspNetCore.Mvc;
using MyDisk.Domain.Models;
using MyDisk.Services.Disks.Command;
using MyDisk.Services.Disks.Queries;

namespace MyDisk.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public partial class DiskController : ControllerBase
    {
        IMediator _mediator;
        public DiskController(IMediator mediator)
        {
            _mediator = mediator;
        }
    }
}
