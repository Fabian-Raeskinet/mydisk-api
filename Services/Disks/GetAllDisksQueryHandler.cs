using AutoMapper;
using MediatR;
using MyDisks.Contracts.Disks;
using MyDisks.Domain;

namespace MyDisks.Services.Disks;

public class GetAllDisksQueryHandler : IQueryHandler<GetAllDisksQueryRequest, IEnumerable<DiskResult>>
{
    public GetAllDisksQueryHandler(IMapper mapper, IDiskRepository repository)
    {
        Mapper = mapper;
        DiskRepository = repository;
    }

    public IMapper Mapper { get; }
    public IDiskRepository DiskRepository { get; }

    public async Task<IEnumerable<DiskResult>> Handle(GetAllDisksQueryRequest request,
        CancellationToken cancellationToken)
    {
        //await Task.Delay(5000);
        var result = await DiskRepository.GetDisksAsync();
        return Mapper.Map<IEnumerable<DiskResult>>(result);
    }
}