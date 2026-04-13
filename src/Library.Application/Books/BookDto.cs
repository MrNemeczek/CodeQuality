using Library.Application.Authors;

namespace Library.Application.Books;

public sealed record BookDto(
    Guid Id,
    string Title,
    string Isbn,
    int PublishedYear,
    int TotalCopies,
    int AvailableCopies,
    AuthorDto Author);
