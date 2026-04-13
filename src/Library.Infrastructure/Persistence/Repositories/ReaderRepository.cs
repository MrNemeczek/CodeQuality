using Library.Application.Abstractions.Persistence;
using Library.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Library.Infrastructure.Persistence.Repositories;

public sealed class ReaderRepository : IReaderRepository
{
    private readonly LibraryDbContext _dbContext;

    public ReaderRepository(LibraryDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task AddAsync(Reader reader, CancellationToken cancellationToken)
    {
        return _dbContext.Readers.AddAsync(reader, cancellationToken).AsTask();
    }

    public async Task<Reader?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _dbContext.Readers.FirstOrDefaultAsync(reader => reader.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyCollection<Reader>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await _dbContext.Readers
            .OrderBy(reader => reader.LastName)
            .ThenBy(reader => reader.FirstName)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> EmailExistsAsync(string email, Guid? excludedReaderId, CancellationToken cancellationToken)
    {
        var normalizedEmail = email.Trim().ToLower();

        return await _dbContext.Readers.AnyAsync(
            reader => reader.Email == normalizedEmail && (!excludedReaderId.HasValue || reader.Id != excludedReaderId.Value),
            cancellationToken);
    }
}
