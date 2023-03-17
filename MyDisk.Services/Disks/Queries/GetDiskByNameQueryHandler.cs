using AutoMapper;
using MediatR;
using MyDisk.Domain.Interfaces.IRepositories;
using MyDisk.Services.Disks.DTOs;
using MyDisk.Services.Disks.Requests;

namespace MyDisk.Services.Disks.Queries;

public class GetDiskByNameQueryHandler : IRequestHandler<GetDiskByNameRequest, DiskResponse?>
{
    public GetDiskByNameQueryHandler(IMapper mapper, IDiskRepository repository)
    {
        Mapper = mapper;
        DiskRepository = repository;
    }

    public IMapper Mapper { get; }
    public IDiskRepository DiskRepository { get; }

    public async Task<DiskResponse?> Handle(GetDiskByNameRequest request, CancellationToken cancellationToken)
    {
        var data = await DiskRepository.GetDiskByFilterAsync(d => d.Name == request.Name);

        return data == null ? null : Mapper.Map<DiskResponse>(data);
    }
}