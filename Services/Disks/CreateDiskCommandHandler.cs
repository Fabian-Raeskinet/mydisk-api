using MediatR;
using MyDisks.Domain;
using MyDisks.Domain.Entities;

namespace MyDisks.Services.Disks;

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