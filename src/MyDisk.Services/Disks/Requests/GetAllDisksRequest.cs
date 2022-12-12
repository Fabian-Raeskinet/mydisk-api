using MediatR;
using MyDisk.Services.Disks.DTOs;

namespace MyDisk.Services.Disks.Requests
{
    public class GetAllDisksRequest : IRequest<List<DiskEntity>> { }
}
