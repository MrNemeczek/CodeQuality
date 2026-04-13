using Library.Domain.Entities;

namespace Library.Application.Abstractions.Persistence;

public interface IAuthorRepository
{
    Task<Author?> GetByFullNameAsync(string firstName, string lastName, CancellationToken cancellationToken);

    Task AddAsync(Author author, CancellationToken cancellationToken);
}
