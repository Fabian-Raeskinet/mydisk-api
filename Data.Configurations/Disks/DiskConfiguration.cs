using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyDisk.Domain.Entities;

namespace Data.Configurations.Disks;

public class DiskConfiguration : IEntityTypeConfiguration<Disk>
{
    public void Configure(EntityTypeBuilder<Disk> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .HasValueGenerator<NewIdGenerator>();

        builder.Property(x => x.Name)
            .HasMaxLength(30)
            .IsRequired();

        builder.Property(x => x.ReleaseDate)
            .IsRequired(false);

        builder.Property(x => x.ImageUrl)
            .IsRequired(false);

        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.Property(x => x.UpdatedAt)
            .IsRequired();

        builder.Property(x => x.AuthorId);
    }
}