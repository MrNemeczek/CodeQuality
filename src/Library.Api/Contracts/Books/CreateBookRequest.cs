namespace Library.Api.Contracts.Books;

public sealed record CreateBookRequest(
    string Title,
    string Isbn,
    int PublishedYear,
    int TotalCopies,
    string AuthorFirstName,
    string AuthorLastName);
