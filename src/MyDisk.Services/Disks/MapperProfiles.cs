using AutoMapper;
using MyDisk.Domain.Models;
using MyDisk.Services.Disks.DTOs;

namespace MyDisk.Services.Disks;

public class MapperProfiles : Profile
{
    public MapperProfiles()
    {
        CreateMap<Disk, DiskResponse>();
        CreateMap<Disk, DiskEntity>();
    }
}