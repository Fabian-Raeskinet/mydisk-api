using MediatR;
using MyDisk.Domain.Models;
using MyDisk.Infrastructure.Interfaces.IRepositories;
using MyDisk.Services.Disks.Requests;

namespace MyDisk.Services.Disks.Commands;

public class CreateDiskCommandHandler : IRequestHandler<CreateDiskRequest, Guid>
{
    private readonly IDiskRepository _repository;

    public CreateDiskCommandHandler(IDiskRepository repository)
    {
        _repository = repository;
    }

    public async Task<Guid> Handle(CreateDiskRequest request, CancellationToken cancellationToken)
    {
        if (request.ReleaseDate == null) throw new InvalidOperationException();
        var entity = new Disk
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            ReleaseDate = DateTime.ParseExact(request.ReleaseDate, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture),
            CreatedDateTime = DateTime.Now
        };

       _repository.CreateDisk(entity);

        return entity.Id;
    }
}