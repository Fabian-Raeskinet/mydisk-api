using MediatR;
using MyDisks.Domain;
using MyDisks.Domain.Disks;

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
        var disk = new Disk
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            ReleaseDate = request.ReleaseDate
        };
        
        disk.AddDomainEvent(new NewDiskDomainEvent(disk));

        await DiskRepository.CreateDiskAsync(disk);

        return Unit.Value;
    }
}