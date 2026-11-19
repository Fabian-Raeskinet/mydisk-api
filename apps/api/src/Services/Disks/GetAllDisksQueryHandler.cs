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
        var result = await UnitOfWork.DiskRepository.GetDisksAsync(disk =>
            disk.CreatedByIssuer == request.Owner.Issuer && disk.CreatedBySubject == request.Owner.Subject);
        return Mapper.Map<IEnumerable<DiskResult>>(result);
    }
}