using AutoMapper;
using FluentAssertions;
using MyDisks.Contracts.Disks;
using MyDisks.Domain.Authors;
using MyDisks.Services.Disks;
using MyDisks.Tests.Services;

namespace MyDisks.Services.Tests.Authors;

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
            Pseudonym = author.Pseudonym
        };

        // Act
        var mapper = config.CreateMapper();
        var act = mapper.Map<AuthorResult>(author);

        // Arrange
        act.Should().BeEquivalentTo(expected);
    }
}