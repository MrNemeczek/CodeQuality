using Library.Domain.Entities;

namespace Library.Application.Abstractions.Persistence;

public interface IReaderRepository
{
    Task AddAsync(Reader reader, CancellationToken cancellationToken);

    Task<Reader?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    Task<IReadOnlyCollection<Reader>> GetAllAsync(CancellationToken cancellationToken);

    Task<bool> EmailExistsAsync(string email, Guid? excludedReaderId, CancellationToken cancellationToken);
}
