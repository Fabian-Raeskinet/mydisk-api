using AutoMapper;
using MyDisks.Contracts.Disks;
using MyDisks.Domain.Authors;

namespace MyDisks.Services.Authors;

public class MapperProfiles : Profile
{
    public MapperProfiles()
    {
        CreateMap<Author, AuthorResult>();
    }
}