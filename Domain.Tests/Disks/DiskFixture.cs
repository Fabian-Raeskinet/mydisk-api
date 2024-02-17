using FluentAssertions;
using MyDisks.Domain.Disks;
using MyDisks.Domain.Reviews;
using MyDisks.Tests.Domain;
using Xunit;

namespace MyDisks.Domain.Tests.Disks;

public class DiskFixture
{
    public class CtorFixture
    {
        [Theory]
        [AutoDomainData]
        public void Should_Initialize_Empty_Reviews_List(Name name)
        {
            // Act
            var sut = new Disk { Name = name };

            // Assert
            sut.Reviews.Should().NotBeNull();
        }

        [Theory]
        [AutoDomainData]
        public void Should_Reviews_List_Be_Empty(Name name)
        {
            // Act
            var sut = new Disk { Name = name };

            // Assert
            sut.Reviews.Should().BeEmpty();
        }
    }

    public class AddReviewFixture
    {
        [Theory]
        [AutoDomainData]
        public void Should_Throws_InvalidOperationException_When_Review_Already_Exists(Disk sut, Review review)
        {
            // Arrange
            sut.Reviews.Add(review);

            // Act
            var act = () => sut.AddReview(review);

            // Assert
            act.Should().Throw<InvalidOperationException>();
        }

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