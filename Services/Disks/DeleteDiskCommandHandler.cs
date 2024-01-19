﻿using MediatR;
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

    public async Task<Unit> Handle(DeleteDiskCommandRequest command, CancellationToken cancellationToken)
    {
        var disk = await DiskRepository.GetDiskByFilterAsync(x => x.Id == command.DiskId);

        if (disk == null)
            throw new ObjectNotFoundException();

        await DiskRepository.DeleteDiskAsync(disk);

        return Unit.Value;
    }
}