using Contracts.Disks;
using MediatR;
using MyDisk.Domain.Exceptions;
using MyDisk.Domain.Interfaces.IRepositories;

namespace MyDisk.Services.Disks;

public class DeleteDiskCommandHandler : IRequestHandler<DeleteDiskCommand, Unit>
{
    public DeleteDiskCommandHandler(IDiskRepository repository)
    {
        DiskRepository = repository;
    }

    public IDiskRepository DiskRepository { get; }

    public async Task<Unit> Handle(DeleteDiskCommand request, CancellationToken cancellationToken)
    {
        if (request.Value == null)
            throw new InvalidOperationException();

        var disk = request.Property switch
        {
            DeleteDiskByPropertyEnum.Id => await DiskRepository.GetDiskByFilterAsync(x =>
                x.Id == new Guid(request.Value)),
            DeleteDiskByPropertyEnum.Name => await DiskRepository.GetDiskByFilterAsync(x => x.Name == request.Value),
            _ => throw new InvalidOperationException()
        };

        if (disk == null)
            throw new ObjectNotFoundException();

        await DiskRepository.DeleteDiskAsync(disk);

        return Unit.Value;
    }
}