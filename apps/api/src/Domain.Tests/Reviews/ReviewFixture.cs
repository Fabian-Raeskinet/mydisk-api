using AutoFixture;
using FluentAssertions;
using MyDisks.Domain.Reviews;
using MyDisks.Tests.Domain;
using Xunit;

namespace MyDisks.Domain.Tests.Reviews;

public class ReviewFixture
{
    public class ContentFixture
    {
        [Fact]
        public void Should_Throws_ArgumentNullException_When_Empty_Content()
        {
            // Arrange
            var content = string.Empty;

            // Act
            var act = () => new Review { Content = content };

            // Assert
            act.Should().Throw<ArgumentNullException>();
        }

        // [Theory]
        // [AutoDomainData]
        // public void Should_Throws_InvalidOperationException_When_Archived_Review(string content)
        //{
            // Arrange
            // var sut = new Fixture().Build<Review>()
            //     .With(x => x.Status, ReviewStatus.Archived).Create();
            // sut.Archive();
            //
            // // Act
            // var act = () => sut.Content = content;
            //
            // // Assert
            // act.Should().Throw<InvalidOperationException>();
            //     }

        [Theory]
        [AutoDomainData]
        public void Should_Set_Content(string content)
        {
            // Act
            var sut = new Review { Content = content };

            // Assert
            sut.Content.Should().Be(content);
        }

        public class TitleFixture
        {
            [Fact]
            public void Should_Throws_ArgumentNullException_When_Empty_Title()
            {
                // Arrange
                var title = string.Empty;

                // Act
                var act = () => new Review { Title = title };

                // Assert
                act.Should().Throw<ArgumentNullException>();
            }

            //  [Theory]
            //  [AutoDomainData]
            //  public void Should_Throws_InvalidOperationException_When_Archived_Review(Review sut, string title)
            //  {
                // // Arrange
                // sut.Archive();
                //
                // // Act
                // var act = () => sut.Title = title;
                //
                // // Assert
                // act.Should().Throw<InvalidOperationException>();
                //      }

            [Fact]
            public void Should_Throws_ArgumentException_When_Title_Exceeds_30_Characters()
            {
                // Arrange
                var title = new string('a', 31);

                // Act
                var act = () => new Review { Title = title };

                // Assert
                act.Should().Throw<ArgumentException>();
            }

            [Fact]
            public void Should_Set_Title()
            {
                // Arrange
                var title = new string('a', 30);

                // Act
                var sut = new Review { Title = title };

                // Assert
                sut.Title.Should().Be(title);
            }
        }

        public class ArchiveFixture
        {
            [Theory]
            [AutoDomainData]
            public void Should_Set_Review_Status_To_Archived(Review sut)
            {
                // Arrange
                // sut.Status = ReviewStatus.Pending;

                // Act
                sut.Archive();

                // Assert
                sut.Status.Should().Be(ReviewStatus.Archived);
            }
        }

        public class ModerationFixture
        {
            [Fact]
            public void Should_Be_Pending_When_Created()
            {
                new Review().Status.Should().Be(ReviewStatus.Pending);
            }

            [Fact]
            public void Should_Publish_PendingReview()
            {
                var sut = new Review();

                sut.Publish();

                sut.Status.Should().Be(ReviewStatus.Published);
            }

            [Fact]
            public void Should_Reject_PendingReview()
            {
                var sut = new Review();

                sut.Reject();

                sut.Status.Should().Be(ReviewStatus.Rejected);
            }

            [Fact]
            public void Should_Throw_When_Moderating_NonPendingReview()
            {
                var sut = new Review();
                sut.Archive();

                var act = () => sut.Publish();

                act.Should().Throw<InvalidOperationException>();
            }
        }
    }
}