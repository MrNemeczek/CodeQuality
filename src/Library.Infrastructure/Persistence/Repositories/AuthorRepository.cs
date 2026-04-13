using Library.Application.Abstractions.Persistence;
using Library.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Library.Infrastructure.Persistence.Repositories;

public sealed class AuthorRepository : IAuthorRepository
{
    private readonly LibraryDbContext _dbContext;

    public AuthorRepository(LibraryDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Author?> GetByFullNameAsync(string firstName, string lastName, CancellationToken cancellationToken)
    {
        var normalizedFirstName = firstName.Trim().ToUpper();
        var normalizedLastName = lastName.Trim().ToUpper();

        return await _dbContext.Authors
            .FirstOrDefaultAsync(
                author => author.FirstName.ToUpper() == normalizedFirstName && author.LastName.ToUpper() == normalizedLastName,
                cancellationToken);
    }

    public Task AddAsync(Author author, CancellationToken cancellationToken)
    {
        return _dbContext.Authors.AddAsync(author, cancellationToken).AsTask();
    }
}
