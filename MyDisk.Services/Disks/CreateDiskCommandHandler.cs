using System.Globalization;
using Contracts.Disks;
using MediatR;
using MyDisk.Domain;
using MyDisk.Domain.Entities;

namespace MyDisk.Services.Disks;

public class CreateDiskCommandHandler : IRequestHandler<CreateDiskCommand, Guid>
{
    public CreateDiskCommandHandler(IDiskRepository repository)
    {
        DiskRepository = repository;
    }

    public IDiskRepository DiskRepository { get; }

    public async Task<Guid> Handle(CreateDiskCommand request, CancellationToken cancellationToken)
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