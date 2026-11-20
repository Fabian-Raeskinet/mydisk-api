using FluentAssertions;
using MyDisks.Data.Configurations.Outbox;

namespace MyDisks.Data.Tests;

public class OutboxEntryFixture
{
    private static readonly DateTimeOffset Now = new(2026, 1, 1, 0, 0, 0, TimeSpan.Zero);

    private static OutboxEntry CreateEntry() => new()
    {
        Type = "review.created",
        Payload = "{}",
        OccurredOnUtc = Now,
        CorrelationId = "correlation"
    };

    public class MarkFailedFixture
    {
        [Theory]
        [InlineData(1, 5)]
        [InlineData(2, 10)]
        [InlineData(4, 40)]
        [InlineData(7, 300)]
        [InlineData(50, 300)]
        public void Should_BackOffExponentially_UpToFiveMinutes(int failures, int expectedDelayInSeconds)
        {
            var entry = CreateEntry();

            for (var i = 0; i < failures; i++)
                entry.MarkFailed("boom", Now);

            entry.RetryCount.Should().Be(failures);
            entry.NextRetryOnUtc.Should().Be(Now.AddSeconds(expectedDelayInSeconds));
            entry.LastError.Should().Be("boom");
        }

        [Fact]
        public void Should_TruncateLastError()
        {
            var entry = CreateEntry();

            entry.MarkFailed(new string('x', OutboxEntry.ErrorMaxLength + 10), Now);

            entry.LastError.Should().HaveLength(OutboxEntry.ErrorMaxLength);
        }
    }

    public class MarkProcessedFixture
    {
        [Fact]
        public void Should_ClearRetryState()
        {
            var entry = CreateEntry();
            entry.MarkFailed("boom", Now);

            entry.MarkProcessed(Now);

            entry.ProcessedOnUtc.Should().Be(Now);
            entry.NextRetryOnUtc.Should().BeNull();
            entry.LastError.Should().BeNull();
        }
    }
}
