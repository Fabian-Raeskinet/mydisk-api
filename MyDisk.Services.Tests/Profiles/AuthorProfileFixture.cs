using AutoMapper;
using FluentAssertions;
using MyDisk.Contracts.Disks;
using MyDisk.Domain.Entities;
using MyDisk.Services.Disks;
using MyDisk.Tests.Services;

namespace MyDisk.Services.Tests.Profiles;

public class AuthorProfileFixture
{
    [Theory]
    [AutoServiceData]
    public void TestDiskResponseMapping(Author author)
    {
        // Arrange
        var config = new MapperConfiguration(cfg => { cfg.AddMaps(typeof(MapperProfiles)); });
        config.AssertConfigurationIsValid();
        var expected = new AuthorResponse
        {
            Id = author.Id,
            Pseudonyme = author.Pseudonyme
        };

        // Act
        var mapper = config.CreateMapper();
        var act = mapper.Map<AuthorResponse>(author);

        // Arrange
        act.Should().BeEquivalentTo(expected);
    }
}