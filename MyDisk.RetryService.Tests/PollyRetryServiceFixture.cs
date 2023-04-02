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
        public async Task ShouldNotRetry(PollyRetryService sut)
        {
            sut.Settings.AsMock()
                .Setup(c => c.Value)
                .Returns(new RetryServiceSettings { RetryCount = 3, TimeRetryMilliSeconds = 1 });

            var retryCount = 0;
            var result = await sut.ExecuteAsync<InvalidOperationException, int>(() =>
            {
                if (retryCount == 0)
                    return Task.FromResult(retryCount);

                throw new InvalidOperationException();
            });

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
            await sut.ExecuteAsync<InvalidOperationException, int>(() =>
            {
                ++retryCount;
                if (retryCount == 2)
                    return Task.FromResult(retryCount);

                throw new InvalidOperationException();
            });

            retryCount.Should().Be(2);
        }

        [Theory]
        [AutoServiceData]
        public async Task ShouldFailWhenRetryCountExceeded(PollyRetryService sut)
        {
            sut.Settings.AsMock()
                .Setup(c => c.Value)
                .Returns(new RetryServiceSettings { RetryCount = 3, TimeRetryMilliSeconds = 1 });

            var act = async () =>
            {
                await sut.ExecuteAsync<InvalidOperationException, int>(() =>
                    throw new InvalidOperationException("RetryCount exceeded"));
            };

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
