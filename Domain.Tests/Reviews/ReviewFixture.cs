using FluentAssertions;
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
        public void Should_Throws_ArgumentNullException_When_Empty_Content(Review sut)
        {
            // Arrange
            var content = string.Empty;
            
            // Act
            var act = () => sut.Content = content;

            // Assert
            act.Should().Throw<ArgumentNullException>();
        }
        
        [Theory]
        [AutoDomainData]
        public void Should_Throws_InvalidOperationException_When_Archived_Review(Review sut, string content)
        {
            // Arrange
            sut.Archive();
            
            // Act
            var act = () => sut.Content = content;

            // Assert
            act.Should().Throw<InvalidOperationException>();
        }
        
        [Theory]
        [AutoDomainData]
        public void Should_Set_Content(Review sut, string content)
        {
            // Arrange
            // sut.Status = ReviewStatus.Pending;
            
            // Act
            sut.Content = content;

            // Assert
            sut.Content.Should().Be(content);
        }

        public class TitleFixture
        {
            [Theory]
            [AutoDomainData]
            public void Should_Throws_ArgumentNullException_When_Empty_Title(Review sut)
            {
                // Arrange
                var title = string.Empty;
                // Act
                var act = () => sut.Title = title;
                // Assert
                act.Should().Throw<ArgumentNullException>();
            }
            
            [Theory]
            [AutoDomainData]
            public void Should_Throws_InvalidOperationException_When_Archived_Review(Review sut, string title)
            {
                // Arrange
                sut.Archive();
                
                // Act
                var act = () => sut.Title = title;
                
                // Assert
                act.Should().Throw<InvalidOperationException>();
            }
            
            [Theory]
            [AutoDomainData]
            public void Should_Throws_ArgumentException_When_Title_Exceeds_30_Characters(Review sut)
            {
                // Arrange
                var title = new string('a', 31);
                
                // Act
                var act = () => sut.Title = title;
                
                // Assert
                act.Should().Throw<ArgumentException>();
            }
            
            [Theory]
            [AutoDomainData]
            public void Should_Set_Title(Review sut)
            {
                // Arrange
                var title = new string('a', 30);
                // sut.Status = ReviewStatus.Pending;
                
                // Act
                sut.Title = title;
                
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
    }
}