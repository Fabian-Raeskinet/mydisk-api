using AutoMapper;
using MediatR;
using MyDisks.Contracts.Disks;
using MyDisks.Domain;
using MyDisks.Domain.Disks;
using MyDisks.Domain.Exceptions;

namespace MyDisks.Services.Disks;

public class GetDiskByNameQueryHandler : IQueryHandler<GetDiskByNameQueryRequest, DiskResult>
{
    public GetDiskByNameQueryHandler(IMapper mapper, IUnitOfWork unitOfWork)
    {
        Mapper = mapper;
        UnitOfWork = unitOfWork;
    }

    public IMapper Mapper { get; }
    public IUnitOfWork UnitOfWork { get; }

    public async Task<DiskResult> Handle(GetDiskByNameQueryRequest request,
        CancellationToken cancellationToken)
    {
        var data = await UnitOfWork.DiskRepository.GetDiskByFilterAsync(d => d.Name == request.Name);

        if (data is null)
            throw new ObjectNotFoundException();

        return Mapper.Map<DiskResult>(data);
    }
}