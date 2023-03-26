using AutoMapper;
using MediatR;
using MyDisk.Contracts.Disks;
using MyDisk.Domain;
using MyDisk.Domain.Exceptions;

namespace MyDisk.Services.Disks;

public class GetDiskByNameQueryHandler : IRequestHandler<GetDiskByNameQueryRequest, DiskResponse>
{
    public GetDiskByNameQueryHandler(IMapper mapper, IDiskRepository repository)
    {
        Mapper = mapper;
        DiskRepository = repository;
    }

    public IMapper Mapper { get; }
    public IDiskRepository DiskRepository { get; }

    public async Task<DiskResponse> Handle(GetDiskByNameQueryRequest request,
        CancellationToken cancellationToken)
    {
        var data = await DiskRepository.GetDiskByFilterAsync(d => d.Name == request.Name);

        if (data == null)
            throw new ObjectNotFoundException();

        return Mapper.Map<DiskResponse>(data);
    }
}