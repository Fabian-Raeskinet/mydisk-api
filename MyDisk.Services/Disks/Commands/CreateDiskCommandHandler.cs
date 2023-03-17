﻿using System.Globalization;
using MediatR;
using MyDisk.Domain.Entities;
using MyDisk.Domain.Interfaces.IRepositories;
using MyDisk.Services.Disks.Requests;

namespace MyDisk.Services.Disks.Commands;

public class CreateDiskCommandHandler : IRequestHandler<CreateDiskRequest, Guid>
{
    public CreateDiskCommandHandler(IDiskRepository repository)
    {
        DiskRepository = repository;
    }

    public IDiskRepository DiskRepository { get; }

    public async Task<Guid> Handle(CreateDiskRequest request, CancellationToken cancellationToken)
    {
        if (request.ReleaseDate == null) throw new InvalidOperationException();
        var entity = new Disk
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            ReleaseDate = DateTime.ParseExact(request.ReleaseDate, "yyyy-MM-dd",
                CultureInfo.InvariantCulture)
        };

        await DiskRepository.CreateDiskAsync(entity);

        return entity.Id;
    }
}