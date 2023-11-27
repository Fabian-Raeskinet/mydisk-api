using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyDisks.Domain.Entities;

namespace MyDisks.Data.Configurations.Authors;

public class AuthorConfiguration : IEntityTypeConfiguration<Author>
{
    public void Configure(EntityTypeBuilder<Author> builder)
    {
        builder.HasIndex(x => x.Id);
        builder.Property(x => x.Id)
            .HasValueGenerator<NewIdGenerator>();

        builder.Property(x => x.Pseudonyme)
            .IsRequired(false);

        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.Property(x => x.UpdatedAt)
            .IsRequired();
    }
}