using AutoMapper;
using MediatR;
using MyDisk.Domain.Models;
using MyDisk.Infrastructure.Persistence;
using MyDisk.Services.Authors.DTOs;
using MyDisk.Services.Disks.DTOs;

namespace MyDisk.Services.Disks.Queries
{
    public class GetDiskByNameQuery : IRequest<DiskResponse>
    {
        public string? Name { get; set; }
    }

    public class Handler : IRequestHandler<GetDiskByNameQuery, DiskResponse>
    {
        private readonly IMapper _mapper;
        public Handler(IMapper mapper)
        {
            _mapper = mapper;
        }


        public async Task<DiskResponse?>? Handle(GetDiskByNameQuery request, CancellationToken cancellationToken)
        {
            var data = StaticContent.DiskData.Where(d => d.Name == request.Name).FirstOrDefault();
            if (data != null)
            {
                return _mapper.Map<DiskResponse>(data);
            }
            return null;
        }
    }
}
