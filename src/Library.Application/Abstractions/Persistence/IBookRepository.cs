using Library.Domain.Entities;

namespace Library.Application.Abstractions.Persistence;

public interface IBookRepository
{
    Task AddAsync(Book book, CancellationToken cancellationToken);

    Task<Book?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    Task<IReadOnlyCollection<Book>> GetAllAsync(CancellationToken cancellationToken);

    Task<bool> IsbnExistsAsync(string isbn, Guid? excludedBookId, CancellationToken cancellationToken);

    void Remove(Book book);
}
