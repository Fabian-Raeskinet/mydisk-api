using AutoMapper;
using Contracts.Disks;
using MediatR;
using MyDisk.Domain;

namespace MyDisk.Services.Disks;

public class GetAllDisksQueryHandler : IRequestHandler<GetAllDisksQuery, List<DiskEntity>>
{
    public GetAllDisksQueryHandler(IMapper mapper, IDiskRepository repository)
    {
        Mapper = mapper;
        DiskRepository = repository;
    }

    public IMapper Mapper { get; }
    public IDiskRepository DiskRepository { get; }

    public async Task<List<DiskEntity>> Handle(GetAllDisksQuery request, CancellationToken cancellationToken)
    {
        //await Task.Delay(5000);
        var result = await DiskRepository.GetDisksAsync();
        return Mapper.Map<List<DiskEntity>>(result);
    }
}