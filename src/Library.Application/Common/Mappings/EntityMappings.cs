using Library.Application.Authors;
using Library.Application.Books;
using Library.Application.Loans;
using Library.Application.Readers;
using Library.Domain.Entities;

namespace Library.Application.Common.Mappings;

public static class EntityMappings
{
    public static AuthorDto ToDto(this Author author)
    {
        return new AuthorDto(author.Id, author.FirstName, author.LastName, author.FullName);
    }

    public static BookDto ToDto(this Book book, int activeLoansCount)
    {
        var author = book.Author ?? throw new InvalidOperationException("Book author was not loaded.");
        return book.ToDto(author, activeLoansCount);
    }

    public static BookDto ToDto(this Book book, Author author, int activeLoansCount)
    {
        var availableCopies = Math.Max(book.TotalCopies - activeLoansCount, 0);
        return new BookDto(book.Id, book.Title, book.Isbn, book.PublishedYear, book.TotalCopies, availableCopies, author.ToDto());
    }

    public static ReaderDto ToDto(this Reader reader)
    {
        return new ReaderDto(reader.Id, reader.FirstName, reader.LastName, reader.FullName, reader.Email);
    }

    public static LoanDto ToDto(this Loan loan)
    {
        var book = loan.Book ?? throw new InvalidOperationException("Loan book was not loaded.");
        var reader = loan.Reader ?? throw new InvalidOperationException("Loan reader was not loaded.");
        return loan.ToDto(book, reader);
    }

    public static LoanDto ToDto(this Loan loan, Book book, Reader reader)
    {
        var author = book.Author ?? throw new InvalidOperationException("Book author was not loaded.");

        return new LoanDto(
            loan.Id,
            new LoanBookDto(book.Id, book.Title, book.Isbn, author.FullName),
            new LoanReaderDto(reader.Id, reader.FullName, reader.Email),
            loan.BorrowedAtUtc,
            loan.DueDateUtc,
            loan.ReturnedAtUtc,
            loan.IsReturned);
    }
}
