using Library.Application.Abstractions.Persistence;
using Library.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Library.Infrastructure.Persistence.Repositories;

public sealed class BookRepository : IBookRepository
{
    private readonly LibraryDbContext _dbContext;

    public BookRepository(LibraryDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task AddAsync(Book book, CancellationToken cancellationToken)
    {
        return _dbContext.Books.AddAsync(book, cancellationToken).AsTask();
    }

    public async Task<Book?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _dbContext.Books
            .Include(book => book.Author)
            .Include(book => book.Loans)
            .FirstOrDefaultAsync(book => book.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyCollection<Book>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await _dbContext.Books
            .Include(book => book.Author)
            .Include(book => book.Loans)
            .OrderBy(book => book.Title)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> IsbnExistsAsync(string isbn, Guid? excludedBookId, CancellationToken cancellationToken)
    {
        var normalizedIsbn = isbn.Trim().ToUpper();

        return await _dbContext.Books.AnyAsync(
            book => book.Isbn == normalizedIsbn && (!excludedBookId.HasValue || book.Id != excludedBookId.Value),
            cancellationToken);
    }

    public void Remove(Book book)
    {
        _dbContext.Books.Remove(book);
    }
}
