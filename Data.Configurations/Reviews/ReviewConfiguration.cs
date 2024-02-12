using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyDisks.Domain.Reviews;

namespace MyDisks.Data.Configurations.Reviews;

public class ReviewConfiguration : IEntityTypeConfiguration<Review>
{
    public void Configure(EntityTypeBuilder<Review> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .HasValueGenerator<NewIdGenerator>();

        builder.Property(x => x.Title)
            .HasMaxLength(30);

        builder.Property(x => x.Content);

        builder.Property(x => x.Note);
        builder.Property(x => x.PublishedDate);
        builder.Property(x => x.Status);
    }
}