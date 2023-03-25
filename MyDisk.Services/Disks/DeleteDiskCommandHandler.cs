using MediatorExtension.Disks;
using MediatR;
using MyDisk.Contracts.Disks;
using MyDisk.Domain;
using MyDisk.Domain.Exceptions;

namespace MyDisk.Services.Disks;

public class DeleteDiskCommandHandler : MediatorExtension.RequestHandler<DeleteDiskCommandRequest, Unit>
{
    public DeleteDiskCommandHandler(IDiskRepository repository)
    {
        DiskRepository = repository;
    }

    public IDiskRepository DiskRepository { get; }

    public override async Task<Unit> Handle(DeleteDiskCommandRequest request, CancellationToken cancellationToken)
    {
        if (request.Value.Value == null)
            throw new InvalidOperationException();

        var disk = request.Value.Property switch
        {
            DeleteDiskByProperty.Id => await DiskRepository.GetDiskByFilterAsync(x =>
                x.Id == new Guid(request.Value.Value)),
            DeleteDiskByProperty.Name => await DiskRepository.GetDiskByFilterAsync(x => x.Name == request.Value.Value),
            _ => throw new InvalidOperationException()
        };

        if (disk == null)
            throw new ObjectNotFoundException();

        await DiskRepository.DeleteDiskAsync(disk);

        return Unit.Value;
    }
}