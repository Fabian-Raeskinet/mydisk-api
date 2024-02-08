using MyDisks.Domain.Reviews;
using MyDisks.Tests.Domain;
using Xunit;

namespace MyDisk.Domain.Tests.Reviews;

public class ReviewFixture
{
    public class ContentFixture
    {
        [Theory]
        [AutoDomainData]
        public void Should_Not_Add_When_Empty(Review review)
        {
            // Arrange
            var value = string.Empty;
            
            // Act
            review.Content = value;

            // Assert
        }
    }
}