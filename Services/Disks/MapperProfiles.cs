using AutoMapper;
using MyDisks.Contracts.Disks;
using MyDisks.Domain.Entities;

namespace MyDisks.Services.Disks;

public class MapperProfiles : Profile
{
    public MapperProfiles()
    {
        CreateMap<Disk, DiskResult>();
    }
}