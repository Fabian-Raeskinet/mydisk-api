using AutoMapper;
using MediatorExtension.Disks;
using MediatR;
using MyDisk.Domain;
using MyDisk.Domain.Exceptions;

namespace MyDisk.Services.Disks;

public class UpdateDiskCommandHandler : MediatorExtension.RequestHandler<UpdateDiskCommandRequest, Unit>
{
    public UpdateDiskCommandHandler(IDiskRepository repository, IMapper mapper)
    {
        DiskRepository = repository;
        Mapper = mapper;
    }

    public IDiskRepository DiskRepository { get; }
    public IMapper Mapper { get; }

    public override async Task<Unit> Handle(UpdateDiskCommandRequest request, CancellationToken cancellationToken)
    {
        var disk = await DiskRepository.GetDiskByFilterAsync(x => request.Value.Id != null && x.Id == request.Value.Id);

        if (disk == null)
            throw new ObjectNotFoundException();

        if (request.Value.Name is not null) disk.Name = request.Value.Name;
        if (request.Value.ReleaseDate is not null) disk.ReleaseDate = request.Value.ReleaseDate;

        await DiskRepository.UpdateDiskAsync(disk);
        return Unit.Value;
    }
}