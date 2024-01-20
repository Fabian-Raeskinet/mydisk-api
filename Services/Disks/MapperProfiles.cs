using AutoMapper;
using MyDisks.Contracts.Disks;
using MyDisks.Domain.Disks;

namespace MyDisks.Services.Disks;

public class MapperProfiles : Profile
{
    public MapperProfiles()
    {
        CreateMap<Disk, DiskResult>();
    }
}