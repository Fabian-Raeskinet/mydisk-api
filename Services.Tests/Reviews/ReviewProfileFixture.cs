using AutoMapper;
using FluentAssertions;
using MyDisks.Contracts.Disks;
using MyDisks.Contracts.Reviews;
using MyDisks.Domain.Reviews;
using MyDisks.Services.Reviews;
using MyDisks.Tests.Services;
using ReviewStatus = MyDisks.Contracts.Reviews.ReviewStatus;

namespace MyDisks.Services.Tests.Reviews;

public class ReviewProfileFixture
{
    [Theory]
    [AutoServiceData]
    public void TestDiskResultMapping(Review review)
    {
        // Arrange
        var config = new MapperConfiguration(cfg => { cfg.AddMaps(typeof(MapperProfiles)); });
        config.AssertConfigurationIsValid();
        var expected = new ReviewResult
        {
           Id = review.Id,
           Title = review.Title,
           Content = review.Content,
           Note = review.Note,
           Status = ReviewStatus.New,
           PublishedDate = review.PublishedDate
        };

        // Act
        var mapper = config.CreateMapper();
        var act = mapper.Map<ReviewResult>(review);

        // Arrange
        act.Should().BeEquivalentTo(expected);
    }
}