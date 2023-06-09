using FluentAssertions;
using MyDisk.Tests.Services;
using MyDisk.Tests.Utils;

namespace MyDisk.RetryService.Tests;

public class PollyRetryServiceFixture
{
    public class ExecuteAsyncWithException
    {
        [Theory]
        [AutoServiceData]
        public async Task Should_Not_Retry(PollyRetryService sut)
        {
            // Arrange
            sut.Settings.AsMock()
                .Setup(c => c.Value)
                .Returns(new RetryServiceSettings { RetryCount = 3, TimeRetryMilliSeconds = 1 });

            var retryCount = 0;
            
            // Act
            var act = await sut.ExecuteAsync<InvalidOperationException, int>(() =>
            {
                if (retryCount == 0)
                    return Task.FromResult(retryCount);

                throw new InvalidOperationException();
            });

            act.Should().Be(0);
        }

        [Theory]
        [AutoServiceData]
        public async Task Should_Retry_Twice(PollyRetryService sut)
        {
            // Arrange
            sut.Settings.AsMock()
                .Setup(c => c.Value)
                .Returns(new RetryServiceSettings { RetryCount = 3, TimeRetryMilliSeconds = 1 });

            var retryCount = 0;
            
            // Act
            await sut.ExecuteAsync<InvalidOperationException, int>(() =>
            {
                ++retryCount;
                if (retryCount == 2)
                    return Task.FromResult(retryCount);

                throw new InvalidOperationException();
            });

            // Assert
            retryCount.Should().Be(2);
        }

        [Theory]
        [AutoServiceData]
        public async Task Should_Fail_When_RetryCount_Exceeded(PollyRetryService sut)
        {
            // Arrange
            sut.Settings.AsMock()
                .Setup(c => c.Value)
                .Returns(new RetryServiceSettings { RetryCount = 3, TimeRetryMilliSeconds = 1 });

            // Act
            var act = async () =>
            {
                await sut.ExecuteAsync<InvalidOperationException, int>(() =>
                    throw new InvalidOperationException("RetryCount exceeded"));
            };

            // Assert
            await act.Should().ThrowAsync<InvalidOperationException>();
        }
    }

    public class ExecuteAsyncWithPredicate
    {
        [Theory]
        [AutoServiceData]
        public async Task ShouldNotRetry(PollyRetryService sut)
        {
            sut.Settings.AsMock()
                .Setup(c => c.Value)
                .Returns(new RetryServiceSettings { RetryCount = 3, TimeRetryMilliSeconds = 1 });

            var retryCount = 0;
            var result = await sut.ExecuteAsync(c => false, () => Task.FromResult(retryCount++));

            result.Should().Be(0);
        }

        [Theory]
        [AutoServiceData]
        public async Task ShouldRetryTwice(PollyRetryService sut)
        {
            sut.Settings.AsMock()
                .Setup(c => c.Value)
                .Returns(new RetryServiceSettings { RetryCount = 3, TimeRetryMilliSeconds = 1 });

            var retryCount = 0;
            var result = await sut.ExecuteAsync(c => c <= 1, () => Task.FromResult(retryCount++));

            result.Should().Be(2);
        }

        [Theory]
        [AutoServiceData]
        public async Task ShouldFailWhenRetryCountExceeded(PollyRetryService sut)
        {
            sut.Settings.AsMock()
                .Setup(c => c.Value)
                .Returns(new RetryServiceSettings { RetryCount = 3, TimeRetryMilliSeconds = 1 });

            var retryCount = 0;
            var result = await sut.ExecuteAsync(c => true, () => Task.FromResult(retryCount++));

            result.Should().Be(3);
        }
    }
}
