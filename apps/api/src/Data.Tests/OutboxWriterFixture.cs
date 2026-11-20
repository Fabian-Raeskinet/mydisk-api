using System.Diagnostics;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using MyDisks.Data.Configurations.Outbox;
using MyDisks.Data.Messaging;

namespace MyDisks.Data.Tests;

public class OutboxWriterFixture
{
    public class EnqueueFixture
    {
        private static ApplicationDbContext CreateContext() => new(
            new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options);

        [Fact]
        public void Should_UseCurrentTraceId_AsCorrelationId()
        {
            using var context = CreateContext();
            var sut = new OutboxWriter(context);
            using var activity = new Activity("test").SetIdFormat(ActivityIdFormat.W3C).Start();

            sut.Enqueue("review.created", new { Id = 1 });

            var entry = context.ChangeTracker.Entries<OutboxEntry>().Single().Entity;
            entry.CorrelationId.Should().Be(activity.TraceId.ToHexString());
            entry.Payload.Should().Be("""{"id":1}""");
        }

        [Fact]
        public void Should_GenerateCorrelationId_WithoutCurrentActivity()
        {
            using var context = CreateContext();
            var sut = new OutboxWriter(context);
            Activity.Current = null;

            sut.Enqueue("review.created", new { Id = 1 });

            var entry = context.ChangeTracker.Entries<OutboxEntry>().Single().Entity;
            Guid.TryParseExact(entry.CorrelationId, "N", out _).Should().BeTrue();
        }
    }
}
