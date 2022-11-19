using AutoMapper;
using MyDisk.Domain.Models;
using MyDisk.Services.Authors.DTOs;

namespace MyDisk.Services.Authors
{
    public class MapperProfiles : Profile
    {
        public MapperProfiles() {
            CreateMap<Author, AuthorResponse>();
        }
    }
}
