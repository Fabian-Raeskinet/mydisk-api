using FluentAssertions;
using MyDisks.Domain.Disks;
using Xunit;

namespace MyDisk.Domain.Tests.Disks;

public class NameFixture
{
    public class CtorFixture
    {
        [Fact]
        public void Should_Throws_ArgumentException_When_More_Than_30_Characters()
        {
            // Arrange
            var name = new string('a', 31);
            
            // Act
            var act = () => new Name(name);

            // Assert
            act.Should().Throw<ArgumentException>();
        }
        
        [Fact]
        public void Should_Set_Name()
        {
            // Arrange
            var name = new string('a', 30);
            
            // Act
            var act = new Name(name);

            // Assert
            act.Value.Should().Be(name);
        }
    }

    public class ExplicitNameFixture
    {
        [Fact]
        public void Should_Convert_Explicitly_Name()
        {
            // Arrange
            var name = new string('a', 30);
            
            // Act
            var act = (Name)name;
            
            // Assert
            act.Value.Should().Be(name);
        }
    }
}