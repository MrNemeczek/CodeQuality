using Library.Application.Abstractions.Messaging;
using Library.Application.Abstractions.Persistence;
using Library.Application.Books;
using Library.Application.Common.Exceptions;
using Library.Application.Common.Mappings;

namespace Library.Application.Books.Queries.GetBookById;

public sealed record GetBookByIdQuery(Guid BookId) : IQuery<BookDto>;

public sealed class GetBookByIdQueryHandler : IQueryHandler<GetBookByIdQuery, BookDto>
{
    private readonly IBookRepository _bookRepository;

    public GetBookByIdQueryHandler(IBookRepository bookRepository)
    {
        _bookRepository = bookRepository;
    }

    public async Task<BookDto> HandleAsync(GetBookByIdQuery query, CancellationToken cancellationToken)
    {
        var book = await _bookRepository.GetByIdAsync(query.BookId, cancellationToken)
            ?? throw new NotFoundException($"Book with id '{query.BookId}' was not found.");

        return book.ToDto(book.Loans.Count(loan => !loan.IsReturned));
    }
}
