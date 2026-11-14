using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyDisks.Domain.Disks;
using MyDisks.Domain.Reviews;

namespace MyDisks.Data.Configurations.Disks;

public class DiskConfiguration : IEntityTypeConfiguration<Disk>
{
    public void Configure(EntityTypeBuilder<Disk> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .HasValueGenerator<NewIdGenerator>();

        builder.Property(x => x.Name)
            .HasConversion(
                v => v.Value,
                v => new Name(v))
            .HasMaxLength(30)
            .IsRequired();

        builder.Property(x => x.ReleaseDate)
            .IsRequired(false);

        builder.Property(x => x.ImageUrl)
            .IsRequired(false);

        builder.Property(x => x.AuthorId);
        builder.HasOne(x => x.Author)
            .WithMany()
            .HasForeignKey(x => x.AuthorId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.SetNull);
    }
}