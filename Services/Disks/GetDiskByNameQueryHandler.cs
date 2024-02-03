using AutoMapper;
using MediatR;
using MyDisks.Contracts.Disks;
using MyDisks.Domain;
using MyDisks.Domain.Exceptions;

namespace MyDisks.Services.Disks;

public class GetDiskByNameQueryHandler : IQueryHandler<GetDiskByNameQueryRequest, DiskResult>
{
    public GetDiskByNameQueryHandler(IMapper mapper, IDiskRepository repository)
    {
        Mapper = mapper;
        DiskRepository = repository;
    }

    public IMapper Mapper { get; }
    public IDiskRepository DiskRepository { get; }

    public async Task<DiskResult> Handle(GetDiskByNameQueryRequest request,
        CancellationToken cancellationToken)
    {
        var data = await DiskRepository.GetDiskByFilterAsync(d => d.Name == request.Name);

        if (data == null)
            throw new ObjectNotFoundException();

        return Mapper.Map<DiskResult>(data);
    }
}