using MediatR;
using MyDisk.Domain.Models;

namespace MyDisk.Services.Disks.Requests
{
    public class GetAllDisksRequest : IRequest<List<Disk>> { }
}
