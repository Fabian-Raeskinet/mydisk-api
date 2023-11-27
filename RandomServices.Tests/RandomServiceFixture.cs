using FluentAssertions;
using MyDisks.RandomServices;
using MyDisks.Tests.Services;

namespace RandomServices.Tests;

public class RandomServiceFixture
{
    public class GetRandomValueFixture
    {
        [Theory]
        [AutoServiceData]
        public void Should_Returns_Random(RandomService sut)
        {
            // Act
            var act = sut.GetRandomValue(1, 1);

            act.Should().Be(1);
        }
    }
}