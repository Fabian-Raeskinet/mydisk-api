using MediatR;
using MyDisk.Domain.Exceptions;
using MyDisk.Domain.Interfaces.IRepositories;
using MyDisk.Services.Common.Enums;
using MyDisk.Services.Disks.Requests;

namespace MyDisk.Services.Disks.Commands;

public class DeleteDiskCommandHandler : IRequestHandler<DeleteDiskRequest, Unit>
{
    public DeleteDiskCommandHandler(IDiskRepository repository)
    {
        DiskRepository = repository;
    }

    public IDiskRepository DiskRepository { get; }

    public async Task<Unit> Handle(DeleteDiskRequest request, CancellationToken cancellationToken)
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