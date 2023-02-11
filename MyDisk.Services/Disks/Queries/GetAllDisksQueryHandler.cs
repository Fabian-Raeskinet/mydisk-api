using AutoMapper;
using MediatR;
using MyDisk.Domain.Interfaces.IRepositories;
using MyDisk.Services.Disks.DTOs;
using MyDisk.Services.Disks.Requests;

namespace MyDisk.Services.Disks.Queries;

public class GetAllDisksQueryHandler : IRequestHandler<GetAllDisksRequest, List<DiskEntity>>
{
    private readonly IMapper _mapper;
    private readonly IDiskRepository _repository;

    public GetAllDisksQueryHandler(IMapper mapper, IDiskRepository repository)
    {
        _mapper = mapper;
        _repository = repository;
    }

    public async Task<List<DiskEntity>> Handle(GetAllDisksRequest request, CancellationToken cancellationToken)
    {
        //await Task.Delay(5000);
        var result = await _repository.GetDisksAsync();
        return _mapper.Map<List<DiskEntity>>(result);
    }
}