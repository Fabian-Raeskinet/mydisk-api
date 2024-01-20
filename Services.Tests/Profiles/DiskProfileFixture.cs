using AutoMapper;
using FluentAssertions;
using MyDisks.Contracts.Disks;
using MyDisks.Domain.Disks;
using MyDisks.Services.Disks;
using MyDisks.Tests.Services;

namespace MyDisks.Services.Tests.Profiles;

public class DiskProfileFixture
{
    [Theory]
    [AutoServiceData]
    public void TestDiskResultMapping(Disk disk)
    {
        // Arrange
        var config = new MapperConfiguration(cfg => { cfg.AddMaps(typeof(MapperProfiles)); });
        config.AssertConfigurationIsValid();
        if (disk.Author == null) return;
        var expected = new DiskResult
        {
            Id = disk.Id,
            Name = disk.Name,
            ReleaseDate = disk.ReleaseDate,
            Author = new AuthorResult
            {
                Id = disk.Author.Id,
                Pseudonyme = disk.Author.Pseudonyme
            },
            ImageUrl = disk.ImageUrl
        };

        // Act
        var mapper = config.CreateMapper();
        var act = mapper.Map<DiskResult>(disk);

        // Arrange
        act.Should().BeEquivalentTo(expected);
    }
}