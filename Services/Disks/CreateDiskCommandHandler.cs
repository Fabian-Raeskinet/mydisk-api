using MediatR;
using MyDisks.Domain;
using MyDisks.Domain.Disks;

namespace MyDisks.Services.Disks;

public sealed class CreateDiskCommandHandler : ICommandHandler<CreateDiskCommandRequest>
{
    public CreateDiskCommandHandler(IDiskRepository repository, IDomainEventDispatcher dispatcher)
    {
        DiskRepository = repository;
        Dispatcher = dispatcher;
    }

    public IDiskRepository DiskRepository { get; }
    public IDomainEventDispatcher Dispatcher { get; set; }

    public async Task Handle(CreateDiskCommandRequest request, CancellationToken cancellationToken)
    {
        var disk = new Disk
        {
            Name = new Name(request.Name),
            ReleaseDate = request.ReleaseDate
        };

        var newDiskEvent = new NewDiskCreatedDomainEvent(disk);

        await Dispatcher.Dispatch(newDiskEvent, cancellationToken);

        await DiskRepository.CreateDiskAsync(disk);
    }
}