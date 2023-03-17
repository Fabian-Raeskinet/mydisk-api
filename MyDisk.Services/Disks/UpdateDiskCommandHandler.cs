using AutoMapper;
using Contracts.Disks;
using MediatR;
using MyDisk.Domain.Exceptions;
using MyDisk.Domain.Interfaces.IRepositories;

namespace MyDisk.Services.Disks;

public class UpdateDiskCommandHandler : IRequestHandler<UpdateDiskCommand, DiskResponse>
{
    public UpdateDiskCommandHandler(IDiskRepository repository, IMapper mapper)
    {
        DiskRepository = repository;
        Mapper = mapper;
    }

    public IDiskRepository DiskRepository { get; }
    public IMapper Mapper { get; }

    public async Task<DiskResponse> Handle(UpdateDiskCommand request, CancellationToken cancellationToken)
    {
        var disk = await DiskRepository.GetDiskByFilterAsync(x => x.Id == request.Id);

        if (disk == null)
            throw new ObjectNotFoundException();

        if (request.Name is not null) disk.Name = request.Name;
        if (request.ReleaseDate is not null) disk.ReleaseDate = request.ReleaseDate;

        await DiskRepository.UpdateDiskAsync(disk);

        return Mapper.Map<DiskResponse>(disk);
    }
}