using AutoMapper;
using FluentAssertions;
using MyDisks.Contracts.Disks;
using MyDisks.Domain.Entities;
using MyDisks.Services.Disks;
using MyDisks.Tests.Services;

namespace MyDisks.Services.Tests.Profiles;

public class AuthorProfileFixture
{
    [Theory]
    [AutoServiceData]
    public void TestDiskResponseMapping(Author author)
    {
        // Arrange
        var config = new MapperConfiguration(cfg => { cfg.AddMaps(typeof(MapperProfiles)); });
        config.AssertConfigurationIsValid();
        var expected = new AuthorResult
        {
            Id = author.Id,
            Pseudonyme = author.Pseudonyme
        };

        // Act
        var mapper = config.CreateMapper();
        var act = mapper.Map<AuthorResult>(author);

        // Arrange
        act.Should().BeEquivalentTo(expected);
    }
}