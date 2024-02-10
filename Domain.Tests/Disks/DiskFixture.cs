using FluentAssertions;
using MyDisks.Domain.Disks;
using MyDisks.Domain.Reviews;
using MyDisks.Tests.Domain;
using Xunit;

namespace MyDisk.Domain.Tests.Disks;

public class DiskFixture
{
    public class AddReviewFixture
    {
        [Theory]
        [AutoDomainData]
        public void Should_Throws_InvalidOperationException_When_Invalid_ReleaseDate(Review review, Disk sut)
        {
            // Arrange
            sut.ReleaseDate = DateTime.Now.AddDays(+1);
            
            // Act
           var act = () => sut.AddReview(review);
            
            // Assert
            act.Should().Throw<InvalidOperationException>();
        }
        
        [Theory]
        [AutoDomainData]
        public void Should_Add_Review(Review review, Disk sut)
        {
            // Arrange
            sut.ReleaseDate = DateTime.Now.AddDays(-1);
            
            // Act
            sut.AddReview(review);
            
            // Assert
            sut.Reviews.Should().Contain(review);
        }
    }
}