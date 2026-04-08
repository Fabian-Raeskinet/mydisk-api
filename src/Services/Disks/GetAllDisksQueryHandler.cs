using AutoMapper;
using MediatR;
using MyDisks.Contracts.Disks;
using MyDisks.Domain;
using MyDisks.Domain.Disks;

namespace MyDisks.Services.Disks;

public class GetAllDisksQueryHandler : IQueryHandler<GetAllDisksQueryRequest, IEnumerable<DiskResult>>
{
    public GetAllDisksQueryHandler(IMapper mapper, IUnitOfWork unitOfWork)
    {
        Mapper = mapper;
        UnitOfWork = unitOfWork;
    }

    public IMapper Mapper { get; }
    public IUnitOfWork UnitOfWork { get; }

    public async Task<IEnumerable<DiskResult>> Handle(GetAllDisksQueryRequest request,
        CancellationToken cancellationToken)
    {
        //await Task.Delay(5000);
        var result = await UnitOfWork.DiskRepository.GetDisksAsync();
        return Mapper.Map<IEnumerable<DiskResult>>(result);
    }
}