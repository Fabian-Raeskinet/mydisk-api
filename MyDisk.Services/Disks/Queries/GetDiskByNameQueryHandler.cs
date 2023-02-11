using AutoMapper;
using MediatR;
using MyDisk.Domain.Interfaces.IRepositories;
using MyDisk.Services.Disks.DTOs;
using MyDisk.Services.Disks.Requests;

namespace MyDisk.Services.Disks.Queries;

public class GetDiskByNameQueryHandler : IRequestHandler<GetDiskByNameRequest, DiskResponse?>
{
    private readonly IMapper _mapper;
    private readonly IDiskRepository _repository;

    public GetDiskByNameQueryHandler(IMapper mapper, IDiskRepository repository)
    {
        _mapper = mapper;
        _repository = repository;
    }

    public async Task<DiskResponse?> Handle(GetDiskByNameRequest request, CancellationToken cancellationToken)
    {
        var data = await _repository.GetDiskByFilterAsync(d => d.Name == request.Name);

        return data == null ? null : _mapper.Map<DiskResponse>(data);
    }
}