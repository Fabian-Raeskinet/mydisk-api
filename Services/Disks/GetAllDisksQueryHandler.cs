﻿using AutoMapper;
using MediatR;
using MyDisk.Contracts.Disks;
using MyDisk.Domain;

namespace MyDisk.Services.Disks;

public class GetAllDisksQueryHandler : IRequestHandler<GetAllDisksQueryRequest, IEnumerable<DiskResult>>
{
    public GetAllDisksQueryHandler(IMapper mapper, IDiskRepository repository)
    {
        Mapper = mapper;
        DiskRepository = repository;
    }

    public IMapper Mapper { get; }
    public IDiskRepository DiskRepository { get; }

    public async Task<IEnumerable<DiskResult>> Handle(GetAllDisksQueryRequest request,
        CancellationToken cancellationToken)
    {
        //await Task.Delay(5000);
        var result = await DiskRepository.GetDisksAsync();
        return Mapper.Map<IEnumerable<DiskResult>>(result);
    }
}