using MediatR;
using MyDisk.Domain.Models;
using MyDisk.Infrastructure.Persistence;

namespace MyDisk.Services.Disks.Queries
{
    public class GetDisksQuery : IRequest<List<Disk>>
    {

    }

    public class GetDisksQueryHandler : IRequestHandler<GetDisksQuery, List<Disk>> {
        public GetDisksQueryHandler() { }

        public async Task<List<Disk>> Handle(GetDisksQuery request, CancellationToken cancellationToken)
        {
            return StaticContent.ContextData;
        }
    }
}
