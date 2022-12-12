using AutoMapper;
using MediatR;
using MyDisk.Infrastructure.Persistence;
using MyDisk.Services.Disks.DTOs;
using MyDisk.Services.Disks.Requests;

namespace MyDisk.Services.Disks.Queries
{
    public class GetDisksQueryHandler : IRequestHandler<GetAllDisksRequest, List<DiskEntity>>
    {

        IMapper _mapper;

        public GetDisksQueryHandler(IMapper mapper)
        {
            _mapper = mapper;
        }

        public async Task<List<DiskEntity>> Handle(GetAllDisksRequest request, CancellationToken cancellationToken)
        {
            //await Task.Delay(5000);
            return _mapper.Map<List<DiskEntity>>(StaticContent.DiskData);
        }
    }
}
