using MediatR;
using MyDisk.Domain.Models;
using MyDisk.Infrastructure.Persistence;
using MyDisk.Services.Disks.Requests;

namespace MyDisk.Services.Disks.Queries
{
    public class GetDisksQueryHandler : IRequestHandler<GetAllDisksRequest, List<Disk>> {
        public async Task<List<Disk>> Handle(GetAllDisksRequest request, CancellationToken cancellationToken)
        {
            //await Task.Delay(5000);
            return StaticContent.DiskData;
        }
    }
}
