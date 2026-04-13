using Library.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Library.Infrastructure.Persistence.Configurations;

public sealed class AuthorConfiguration : IEntityTypeConfiguration<Author>
{
    public void Configure(EntityTypeBuilder<Author> builder)
    {
        builder.ToTable("authors");

        builder.HasKey(author => author.Id);

        builder.Property(author => author.FirstName)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(author => author.LastName)
            .HasMaxLength(100)
            .IsRequired();

        builder.Ignore(author => author.FullName);

        builder.HasIndex(author => new { author.FirstName, author.LastName })
            .IsUnique();
    }
}
