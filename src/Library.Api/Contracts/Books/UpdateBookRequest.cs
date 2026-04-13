namespace Library.Api.Contracts.Books;

public sealed record UpdateBookRequest(
    string Title,
    string Isbn,
    int PublishedYear,
    int TotalCopies,
    string AuthorFirstName,
    string AuthorLastName);
