using AutoMapper;
using MediatR;
using MyDisk.Domain.Exceptions;
using MyDisk.Domain.Interfaces.IRepositories;
using MyDisk.Services.Disks.DTOs;
using MyDisk.Services.Disks.Requests;

namespace MyDisk.Services.Disks.Commands;

public class UpdateDiskCommandHandler : IRequestHandler<UpdateDiskRequest, DiskResponse>
{
    public UpdateDiskCommandHandler(IDiskRepository repository, IMapper mapper)
    {
        DiskRepository = repository;
        Mapper = mapper;
    }

    public IDiskRepository DiskRepository { get; }
    public IMapper Mapper { get; }

    public async Task<DiskResponse> Handle(UpdateDiskRequest request, CancellationToken cancellationToken)
    {
        var disk = await DiskRepository.GetDiskByFilterAsync(x => x.Id == request.Id);

        if (disk == null)
            throw new ObjectNotFoundException();

        if (request.Name is not null) disk.Name = request.Name;
        if (request.ReleaseDate is not null) disk.ReleaseDate = request.ReleaseDate;

        await DiskRepository.UpdateDiskAsync(disk);

        return Mapper.Map<DiskResponse>(disk);
    }
}