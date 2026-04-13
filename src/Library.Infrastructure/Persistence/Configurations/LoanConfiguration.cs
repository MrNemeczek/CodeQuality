using Library.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Library.Infrastructure.Persistence.Configurations;

public sealed class LoanConfiguration : IEntityTypeConfiguration<Loan>
{
    public void Configure(EntityTypeBuilder<Loan> builder)
    {
        builder.ToTable("loans");

        builder.HasKey(loan => loan.Id);

        builder.Property(loan => loan.BorrowedAtUtc)
            .IsRequired();

        builder.Property(loan => loan.DueDateUtc)
            .IsRequired();

        builder.Ignore(loan => loan.IsReturned);

        builder.HasOne(loan => loan.Book)
            .WithMany(book => book.Loans)
            .HasForeignKey(loan => loan.BookId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(loan => loan.Reader)
            .WithMany(reader => reader.Loans)
            .HasForeignKey(loan => loan.ReaderId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(loan => new { loan.BookId, loan.ReturnedAtUtc });
        builder.HasIndex(loan => new { loan.ReaderId, loan.BorrowedAtUtc });
    }
}
