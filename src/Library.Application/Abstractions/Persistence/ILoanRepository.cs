using Library.Domain.Entities;

namespace Library.Application.Abstractions.Persistence;

public interface ILoanRepository
{
    Task AddAsync(Loan loan, CancellationToken cancellationToken);

    Task<Loan?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    Task<IReadOnlyCollection<Loan>> GetActiveAsync(CancellationToken cancellationToken);

    Task<IReadOnlyCollection<Loan>> GetByReaderIdAsync(Guid readerId, CancellationToken cancellationToken);
}
