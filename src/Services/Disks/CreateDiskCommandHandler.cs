using MyDisks.Domain;
using MyDisks.Domain.Disks;

namespace MyDisks.Services.Disks;

public sealed class CreateDiskCommandHandler : ICommandHandler<CreateDiskCommandRequest>
{
    public CreateDiskCommandHandler(IDiskRepository repository)
    {
        DiskRepository = repository;
    }

    public IDiskRepository DiskRepository { get; }

    public async Task Handle(CreateDiskCommandRequest request, CancellationToken cancellationToken)
    {
        var disk = new Disk
        {
            Name = new Name(request.Name),
            ReleaseDate = request.ReleaseDate
        };

        var newDiskEvent = new NewDiskCreatedDomainEvent(disk);

        disk.AddDomainEvent(newDiskEvent);
        await DiskRepository.CreateDiskAsync(disk);
    }
}