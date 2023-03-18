using AutoMapper;
using Contracts.Disks;
using FluentAssertions;
using MyDisk.Domain.Entities;
using MyDisk.Services.Disks;
using MyDisk.Tests.Services;

namespace MyDisk.Services.Tests.Profiles;

public class DiskProfileFixture
{
    [Theory]
    [AutoServiceData]
    public void TestDiskResponseMapping(Disk disk)
    {
        // Arrange
        var config = new MapperConfiguration(cfg => { cfg.AddMaps(typeof(MapperProfiles)); });
        config.AssertConfigurationIsValid();
        if (disk.Author == null) return;
        var expected = new DiskResponse
        {
            Name = disk.Name,
            ReleaseDate = disk.ReleaseDate,
            Author = new AuthorResponse
            {
                Id = disk.Author.Id,
                Pseudonyme = disk.Author.Pseudonyme
            }
        };

        // Act
        var mapper = config.CreateMapper();
        var act = mapper.Map<DiskResponse>(disk);

        // Arrange
        act.Should().BeEquivalentTo(expected);
    }
    
    [Theory]
    [AutoServiceData]
    public void TestDiskEntityMapping(Disk disk)
    {
        // Arrange
        var config = new MapperConfiguration(cfg => { cfg.AddMaps(typeof(MapperProfiles)); });
        config.AssertConfigurationIsValid();
        if (disk.Author == null) return;
        var expected = new DiskEntity
        {
            Name = disk.Name,
            ReleaseDate = disk.ReleaseDate,
            Author = new AuthorResponse
            {
                Id = disk.Author.Id,
                Pseudonyme = disk.Author.Pseudonyme,
            },
            Id = disk.Id,
            ImageUrl = disk.ImageUrl
        };

        // Act
        var mapper = config.CreateMapper();
        var act = mapper.Map<DiskEntity>(disk);

        // Arrange
        act.Should().BeEquivalentTo(expected);
    }
}