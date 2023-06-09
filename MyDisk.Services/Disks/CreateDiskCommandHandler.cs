using MediatR;
using MyDisk.Domain;
using MyDisk.Domain.Entities;

namespace MyDisk.Services.Disks;

public class CreateDiskCommandHandler : IRequestHandler<CreateDiskCommandRequest, Unit>
{
    public CreateDiskCommandHandler(IDiskRepository repository)
    {
        DiskRepository = repository;
    }

    public IDiskRepository DiskRepository { get; }

    public async Task<Unit> Handle(CreateDiskCommandRequest request, CancellationToken cancellationToken)
    {
        var entity = new Disk
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            ReleaseDate = request.ReleaseDate
        };

        await DiskRepository.CreateDiskAsync(entity);

        return Unit.Value;
    }
}