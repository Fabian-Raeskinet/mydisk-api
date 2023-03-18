using AutoMapper;
using Contracts.Disks;
using MyDisk.Domain.Entities;

namespace MyDisk.Services.Disks;

public class MapperProfiles : Profile
{
    public MapperProfiles()
    {
        CreateMap<Disk, DiskResponse>();
        CreateMap<Disk, DiskEntity>();
    }
}