using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MyDisks.Data.Configurations.Outbox;

public sealed class OutboxConfiguration : IEntityTypeConfiguration<OutboxEntry>
{
    public void Configure(EntityTypeBuilder<OutboxEntry> builder)
    {
        builder.ToTable("OutboxMessages");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasValueGenerator<NewIdGenerator>();
        builder.Property(x => x.Type).HasMaxLength(OutboxEntry.TypeMaxLength);
        builder.Property(x => x.Payload).HasColumnType("jsonb");
        builder.Property(x => x.CorrelationId).HasMaxLength(OutboxEntry.CorrelationIdMaxLength);
        builder.Property(x => x.LastError).HasMaxLength(OutboxEntry.ErrorMaxLength);

        builder.HasIndex(x => new { x.OccurredOnUtc, x.Id })
            .HasFilter("\"ProcessedOnUtc\" IS NULL");
    }
}
