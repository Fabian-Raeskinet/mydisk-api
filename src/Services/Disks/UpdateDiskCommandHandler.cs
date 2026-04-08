using AutoMapper;
using MediatR;
using MyDisks.Domain;
using MyDisks.Domain.Disks;
using MyDisks.Domain.Exceptions;

namespace MyDisks.Services.Disks;

public class UpdateDiskCommandHandler : ICommandHandler<UpdateDiskCommandRequest>
{
    public UpdateDiskCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        UnitOfWork = unitOfWork;
        Mapper = mapper;
    }

    public IUnitOfWork UnitOfWork { get; }
    public IMapper Mapper { get; }

    public async Task Handle(UpdateDiskCommandRequest request, CancellationToken cancellationToken)
    {
        var disk = await UnitOfWork.DiskRepository.GetDiskByFilterAsync(x => x.Id == request.Id);

        if (disk == null)
            throw new ObjectNotFoundException();

        if (request.Name is not null) disk.Name = new Name(request.Name);
        if (request.ReleaseDate is not null) disk.ReleaseDate = request.ReleaseDate;

        await UnitOfWork.DiskRepository.UpdateDiskAsync(disk);
        await UnitOfWork.CommitAsync();
    }
}