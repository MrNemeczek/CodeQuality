using Library.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Library.Infrastructure.Persistence.Configurations;

public sealed class BookConfiguration : IEntityTypeConfiguration<Book>
{
    public void Configure(EntityTypeBuilder<Book> builder)
    {
        builder.ToTable("books");

        builder.HasKey(book => book.Id);

        builder.Property(book => book.Title)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(book => book.Isbn)
            .HasMaxLength(32)
            .IsRequired();

        builder.Property(book => book.PublishedYear)
            .IsRequired();

        builder.Property(book => book.TotalCopies)
            .IsRequired();

        builder.HasIndex(book => book.Isbn)
            .IsUnique();

        builder.HasOne(book => book.Author)
            .WithMany(author => author.Books)
            .HasForeignKey(book => book.AuthorId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
