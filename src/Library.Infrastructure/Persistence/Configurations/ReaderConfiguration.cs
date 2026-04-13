using Library.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Library.Infrastructure.Persistence.Configurations;

public sealed class ReaderConfiguration : IEntityTypeConfiguration<Reader>
{
    public void Configure(EntityTypeBuilder<Reader> builder)
    {
        builder.ToTable("readers");

        builder.HasKey(reader => reader.Id);

        builder.Property(reader => reader.FirstName)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(reader => reader.LastName)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(reader => reader.Email)
            .HasMaxLength(256)
            .IsRequired();

        builder.Ignore(reader => reader.FullName);

        builder.HasIndex(reader => reader.Email)
            .IsUnique();
    }
}
