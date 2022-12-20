using AutoMapper;
using MediatR;
using MyDisk.Infrastructure.Interfaces.IRepositories;
using MyDisk.Infrastructure.Persistence;
using MyDisk.Services.Disks.DTOs;
using MyDisk.Services.Disks.Requests;

namespace MyDisk.Services.Disks.Queries;

public class GetAllDisksQueryHandler : IRequestHandler<GetDiskByNameRequest, DiskResponse?>
{
    private readonly IMapper _mapper;
    private readonly IDiskRepository _repository;
    public GetAllDisksQueryHandler(IMapper mapper, IDiskRepository repository)
    {
        _mapper = mapper;
        _repository = repository;
    }

    public Task<DiskResponse?> Handle(GetDiskByNameRequest request, CancellationToken cancellationToken)
    {
        var data = _repository.GetDiskByFilter(d => d.Name == request.Name);

        return Task.FromResult(data == null ? null : _mapper.Map<DiskResponse>(data));
    }
}