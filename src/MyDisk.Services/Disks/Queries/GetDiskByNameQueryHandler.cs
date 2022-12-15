using AutoMapper;
using MediatR;
using MyDisk.Infrastructure.Persistence;
using MyDisk.Services.Disks.DTOs;
using MyDisk.Services.Disks.Requests;

namespace MyDisk.Services.Disks.Queries;

public class GetAllDisksQueryHandler : IRequestHandler<GetDiskByNameRequest, DiskResponse?>
{
    private readonly IMapper _mapper;
    public GetAllDisksQueryHandler(IMapper mapper)
    {
        _mapper = mapper;
    }

    public Task<DiskResponse?> Handle(GetDiskByNameRequest request, CancellationToken cancellationToken)
    {
        var data = StaticContent.DiskData.FirstOrDefault(d => d.Name == request.Name);

        return Task.FromResult(data == null ? null : _mapper.Map<DiskResponse>(data));
    }
}