namespace Library.Application.Loans;

public sealed record LoanBookDto(Guid Id, string Title, string Isbn, string AuthorFullName);

public sealed record LoanReaderDto(Guid Id, string FullName, string Email);

public sealed record LoanDto(
    Guid Id,
    LoanBookDto Book,
    LoanReaderDto Reader,
    DateTime BorrowedAtUtc,
    DateTime DueDateUtc,
    DateTime? ReturnedAtUtc,
    bool IsReturned);
