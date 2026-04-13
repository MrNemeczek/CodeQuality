using Library.Application.Abstractions.Messaging;
using Library.Application.Abstractions.Persistence;
using Library.Application.Books;
using Library.Application.Common.Mappings;

namespace Library.Application.Books.Queries.GetBooks;

public sealed record GetBooksQuery : IQuery<IReadOnlyCollection<BookDto>>;

public sealed class GetBooksQueryHandler : IQueryHandler<GetBooksQuery, IReadOnlyCollection<BookDto>>
{
    private readonly IBookRepository _bookRepository;

    public GetBooksQueryHandler(IBookRepository bookRepository)
    {
        _bookRepository = bookRepository;
    }

    public async Task<IReadOnlyCollection<BookDto>> HandleAsync(GetBooksQuery query, CancellationToken cancellationToken)
    {
        var books = await _bookRepository.GetAllAsync(cancellationToken);

        return books
            .Select(book => book.ToDto(book.Loans.Count(loan => !loan.IsReturned)))
            .ToList();
    }
}
