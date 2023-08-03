using MediatR;
using MyDisks.Contracts.Disks;
using MyDisks.Domain;
using MyDisks.Domain.Exceptions;

namespace MyDisks.Services.Disks;

public class DeleteDiskCommandHandler : IRequestHandler<DeleteDiskCommandRequest, Unit>
{
    public DeleteDiskCommandHandler(IDiskRepository repository)
    {
        DiskRepository = repository;
    }

    public IDiskRepository DiskRepository { get; }

    public async Task<Unit> Handle(DeleteDiskCommandRequest request, CancellationToken cancellationToken)
    {
        if (request.Value == null)
            throw new InvalidOperationException();

        var disk = request.Property switch
        {
            DeleteDiskByProperty.Id => await DiskRepository.GetDiskByFilterAsync(x =>
                x.Id == new Guid(request.Value)),
            DeleteDiskByProperty.Name => await DiskRepository.GetDiskByFilterAsync(x => x.Name == request.Value),
            _ => throw new InvalidOperationException()
        };

        if (disk == null)
            throw new ObjectNotFoundException();

        await DiskRepository.DeleteDiskAsync(disk);

        return Unit.Value;
    }
}