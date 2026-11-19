using AutoMapper;
using MyDisks.Contracts.Disks;
using MyDisks.Domain.Disks;
using MyDisks.Domain.Reviews;

namespace MyDisks.Services.Disks;

public class MapperProfiles : Profile
{
    public MapperProfiles()
    {
        CreateMap<Name, string>().ConstructUsing(name => name.Value);
        CreateMap<Disk, DiskResult>()
            .ForMember(result => result.Reviews, options => options.MapFrom(
                disk => disk.Reviews.Where(review => review.Status != ReviewStatus.Archived)));
    }
}