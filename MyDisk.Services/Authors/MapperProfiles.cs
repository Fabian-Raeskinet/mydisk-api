using AutoMapper;
using Contracts.Disks;
using MyDisk.Domain.Entities;

namespace MyDisk.Services.Authors;

public class MapperProfiles : Profile
{
    public MapperProfiles() {
        CreateMap<Author, AuthorResponse>();
    }
}