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

        builder.HasOne(x => x.Disk)
            .WithMany(x => x.Reviews)
            .HasForeignKey(x => x.DiskId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}