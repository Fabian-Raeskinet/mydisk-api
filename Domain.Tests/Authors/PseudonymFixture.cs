using FluentAssertions;
using MyDisks.Domain.Authors;
using MyDisks.Tests.Domain;
using Xunit;

namespace MyDisk.Domain.Tests.Authors;

public class PseudonymFixture
{
    public class CtorFixture
    {
        [Fact]
        public void Should_Throws_ArgumentException()
        {
            // Arrange
            var pseudonym = new string('a', 31);
            
            // Act
            var act = () => new Pseudonym(pseudonym);
            
            // Assert
            act.Should().Throw<ArgumentException>();
        }
        
        [Fact]
        public void Should_Set_Pseudonym()
        {
            // Arrange
            var pseudonym = new string('a', 30);
            
            // Act
            var act = new Pseudonym(pseudonym);
            
            // Assert
            act.Value.Should().Be(pseudonym);
        }
        
        [Theory]
        [AutoDomainData]
        public void Should_Converts_Explicitly_Pseudonym(Pseudonym sut)
        {
            // Arrange
            var pseudonym = new string('a', 30);
            
            // Act
            var act = (Pseudonym)pseudonym;
            
            // Assert
            act.Value.Should().Be(pseudonym);
        }
    }
}