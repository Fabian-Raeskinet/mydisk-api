using AutoMapper;
using MediatR;
using MyDisk.Domain.Models;
using MyDisk.Infrastructure.Persistence;
using MyDisk.Services.Disks.DTOs;
using MyDisk.Services.Disks.Requests;

namespace MyDisk.Services.Disks.Queries
{
    public class GetAllDisksQueryHandler : IRequestHandler<GetDiskByNameRequest, DiskResponse>
    {
        private readonly IMapper _mapper;
        public GetAllDisksQueryHandler(IMapper mapper)
        {
            _mapper = mapper;
        }

        public async Task<DiskResponse?> Handle(GetDiskByNameRequest request, CancellationToken cancellationToken)
        {
            var data = StaticContent.DiskData.Where(d => d.Name == request.Name).FirstOrDefault();

            if (data == null) return null;

            return _mapper.Map<DiskResponse>(data);
        }
    }
}
