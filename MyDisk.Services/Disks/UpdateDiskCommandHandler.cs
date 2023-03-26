using AutoMapper;
using MediatR;
using MyDisk.Domain;
using MyDisk.Domain.Exceptions;

namespace MyDisk.Services.Disks;

public class UpdateDiskCommandHandler : IRequestHandler<UpdateDiskCommandRequest, Unit>
{
    public UpdateDiskCommandHandler(IDiskRepository repository, IMapper mapper)
    {
        DiskRepository = repository;
        Mapper = mapper;
    }

    public IDiskRepository DiskRepository { get; }
    public IMapper Mapper { get; }

    public async Task<Unit> Handle(UpdateDiskCommandRequest request, CancellationToken cancellationToken)
    {
        var disk = await DiskRepository.GetDiskByFilterAsync(x => request.Id != null && x.Id == request.Id);

        if (disk == null)
            throw new ObjectNotFoundException();

        if (request.Name is not null) disk.Name = request.Name;
        if (request.ReleaseDate is not null) disk.ReleaseDate = request.ReleaseDate;

        await DiskRepository.UpdateDiskAsync(disk);
        return Unit.Value;
    }
}