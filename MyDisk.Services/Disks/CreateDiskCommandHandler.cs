using MediatorExtension;
using MyDisk.Contracts.Disks;
using MyDisk.Domain;
using MyDisk.Domain.Entities;

namespace MyDisk.Services.Disks;

public class CreateDiskCommandHandler : RequestHandler<CreateDiskCommand, Guid>
{
    public CreateDiskCommandHandler(IDiskRepository repository)
    {
        DiskRepository = repository;
    }

    public IDiskRepository DiskRepository { get; }

    public override async Task<Guid> Handle(Request<CreateDiskCommand, Guid> request,
        CancellationToken cancellationToken)
    {
        if (request.Value.ReleaseDate == null) throw new InvalidOperationException();
        var entity = new Disk
        {
            Id = Guid.NewGuid(),
            Name = request.Value.Name,
            ReleaseDate = request.Value.ReleaseDate
        };

        await DiskRepository.CreateDiskAsync(entity);

        return entity.Id;
    }
}