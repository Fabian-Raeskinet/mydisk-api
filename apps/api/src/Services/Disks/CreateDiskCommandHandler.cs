using MyDisks.Domain;
using MyDisks.Domain.Disks;

namespace MyDisks.Services.Disks;

public sealed class CreateDiskCommandHandler : ICommandHandler<CreateDiskCommandRequest>
{
    public CreateDiskCommandHandler(IUnitOfWork unitOfWork)
    {
        UnitOfWork = unitOfWork;
    }

    private IUnitOfWork UnitOfWork { get; }

    public async Task Handle(CreateDiskCommandRequest request, CancellationToken cancellationToken)
    {
        var disk = new Disk
        {
            Name = new Name(request.Name),
            ReleaseDate = request.ReleaseDate
        };

        var newDiskEvent = new NewDiskCreatedDomainEvent(disk);

        disk.AddDomainEvent(newDiskEvent);
        await UnitOfWork.DiskRepository.CreateDiskAsync(disk);
        await UnitOfWork.CommitAsync();
    }
}