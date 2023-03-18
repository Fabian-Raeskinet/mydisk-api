using AutoMapper;
using Contracts.Disks;
using MediatR;
using MyDisk.Domain;
using MyDisk.Domain.Exceptions;

namespace MyDisk.Services.Disks;

public class GetDiskByNameQueryHandler : IRequestHandler<GetDiskByNameQuery, DiskResponse?>
{
    public GetDiskByNameQueryHandler(IMapper mapper, IDiskRepository repository)
    {
        Mapper = mapper;
        DiskRepository = repository;
    }

    public IMapper Mapper { get; }
    public IDiskRepository DiskRepository { get; }

    public async Task<DiskResponse?> Handle(GetDiskByNameQuery request, CancellationToken cancellationToken)
    {
        var data = await DiskRepository.GetDiskByFilterAsync(d => d.Name == request.Name);

        if (data == null)
            throw new ObjectNotFoundException();

        return Mapper.Map<DiskResponse>(data);
    }
}