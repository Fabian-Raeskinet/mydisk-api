using AutoMapper;
using MediatR;
using MyDisk.Domain.Interfaces.IRepositories;
using MyDisk.Services.Common.Exceptions;
using MyDisk.Services.Disks.DTOs;
using MyDisk.Services.Disks.Requests;

namespace MyDisk.Services.Disks.Commands;

public class UpdateDiskCommandHandler : IRequestHandler<UpdateDiskRequest, DiskResponse>
{
    private readonly IDiskRepository _repository;
    private readonly IMapper _mapper;

    public UpdateDiskCommandHandler(IDiskRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<DiskResponse> Handle(UpdateDiskRequest request, CancellationToken cancellationToken)
    {
        var disk = await _repository.GetDiskByFilterAsync(x => x.Id == request.Id);

        if (disk == null)
            throw new EntityNotFoundException();

        if (request.Name is not null) disk.Name = request.Name;
        if (request.ReleaseDate is not null) disk.ReleaseDate = request.ReleaseDate;

        await _repository.UpdateDiskAsync(disk);

        return _mapper.Map<DiskResponse>(disk);
    }
}