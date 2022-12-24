using MediatR;
using MyDisk.Domain.Interfaces.IRepositories;
using MyDisk.Services.Common.Enums;
using MyDisk.Services.Common.Exceptions;
using MyDisk.Services.Disks.Requests;

namespace MyDisk.Services.Disks.Commands;

public class DeleteDiskCommandHandler : IRequestHandler<DeleteDiskRequest, Unit>
{
    private readonly IDiskRepository _repository;

    public DeleteDiskCommandHandler(IDiskRepository repository)
    {
        _repository = repository;
    }

    public async Task<Unit> Handle(DeleteDiskRequest request, CancellationToken cancellationToken)
    {
        if (request.Value == null)
            throw new InvalidOperationException();

        var disk = request.Property switch
        {
            DeleteDiskByPropertyEnum.Id => await _repository.GetDiskByFilterAsync(x => x.Id == new Guid(request.Value)),
            DeleteDiskByPropertyEnum.Name => await _repository.GetDiskByFilterAsync(x => x.Name == request.Value),
            _ => throw new InvalidOperationException()
        };

        if (disk == null)
            throw new EntityNotFoundException();

        await _repository.DeleteDiskAsync(disk);

        return Unit.Value;
    }
}