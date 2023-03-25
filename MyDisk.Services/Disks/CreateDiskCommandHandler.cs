using MediatorExtension.Disks;
using MediatR;
using MyDisk.Domain;
using MyDisk.Domain.Entities;

namespace MyDisk.Services.Disks;

public class CreateDiskCommandHandler : MediatorExtension.RequestHandler<CreateDiskCommandRequest, Unit>
{
    public CreateDiskCommandHandler(IDiskRepository repository)
    {
        DiskRepository = repository;
    }

    public IDiskRepository DiskRepository { get; }

    public override async Task<Unit> Handle(CreateDiskCommandRequest request, CancellationToken cancellationToken)
    {
        if (request.Value.ReleaseDate == null) throw new InvalidOperationException();
        var entity = new Disk
        {
            Id = Guid.NewGuid(),
            Name = request.Value.Name,
            ReleaseDate = request.Value.ReleaseDate
        };

        await DiskRepository.CreateDiskAsync(entity);

        return Unit.Value;
    }
}