using AutoMapper;
using MediatR;
using MyDisk.Domain.Interfaces.IRepositories;
using MyDisk.Services.Disks.DTOs;
using MyDisk.Services.Disks.Requests;

namespace MyDisk.Services.Disks.Queries;

public class GetAllDisksQueryHandler : IRequestHandler<GetAllDisksRequest, List<DiskEntity>>
{
    public GetAllDisksQueryHandler(IMapper mapper, IDiskRepository repository)
    {
        Mapper = mapper;
        DiskRepository = repository;
    }

    public IMapper Mapper { get; }
    public IDiskRepository DiskRepository { get; }

    public async Task<List<DiskEntity>> Handle(GetAllDisksRequest request, CancellationToken cancellationToken)
    {
        //await Task.Delay(5000);
        var result = await DiskRepository.GetDisksAsync();
        return Mapper.Map<List<DiskEntity>>(result);
    }
}