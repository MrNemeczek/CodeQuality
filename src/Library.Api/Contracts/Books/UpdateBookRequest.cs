namespace Library.Api.Contracts.Books;

public sealed record UpdateBookRequest
{
    public required string Title { get; init; }

    public required string Isbn { get; init; }

    public required int PublishedYear { get; init; }

    public required int TotalCopies { get; init; }

    public required string AuthorFirstName { get; init; }

    public required string AuthorLastName { get; init; }
}
