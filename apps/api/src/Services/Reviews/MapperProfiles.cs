using AutoMapper;
using MyDisks.Contracts.Reviews;
using MyDisks.Domain.Reviews;

namespace MyDisks.Services.Reviews;

public class MapperProfiles : Profile
{
    public MapperProfiles()
    {
        CreateMap<Review, ReviewResult>();
    }
}