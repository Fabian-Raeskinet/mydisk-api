using MediatR;
using MyDisks.Domain;
using MyDisks.Domain.Disks;

namespace MyDisks.Services.Disks;

public class CreateDiskCommandHandler : IRequestHandler<CreateDiskCommandRequest, Unit>
{
    public CreateDiskCommandHandler(IDiskRepository repository, IDomainEventDispatcher dispatcher)
    {
        DiskRepository = repository;
        Dispatcher = dispatcher;
    }

    public IDiskRepository DiskRepository { get; }
    public IDomainEventDispatcher Dispatcher { get; set; }

    public async Task<Unit> Handle(CreateDiskCommandRequest request, CancellationToken cancellationToken)
    {
        var disk = new Disk
        {
            Name = request.Name,
            ReleaseDate = request.ReleaseDate
        };

        var newDiskEvent = new NewDiskDomainEvent(disk);

        await Dispatcher.Dispatch(newDiskEvent);

        await DiskRepository.CreateDiskAsync(disk);

        return Unit.Value;
    }
}