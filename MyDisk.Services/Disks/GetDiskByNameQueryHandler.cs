using AutoMapper;
using MediatorExtension;
using MyDisk.Contracts.Disks;
using MyDisk.Domain;
using MyDisk.Domain.Exceptions;

namespace MyDisk.Services.Disks;

public class GetDiskByNameQueryHandler : RequestHandler<GetDiskByNameQuery, DiskResponse>
{
    public GetDiskByNameQueryHandler(IMapper mapper, IDiskRepository repository)
    {
        Mapper = mapper;
        DiskRepository = repository;
    }

    public IMapper Mapper { get; }
    public IDiskRepository DiskRepository { get; }

    public override async Task<DiskResponse> Handle(Request<GetDiskByNameQuery, DiskResponse> request,
        CancellationToken cancellationToken)
    {
        var data = await DiskRepository.GetDiskByFilterAsync(d => d.Name == request.Value.Name);

        if (data == null)
            throw new ObjectNotFoundException();

        return Mapper.Map<DiskResponse>(data);
    }
}