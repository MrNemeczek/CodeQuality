using Library.Application.Abstractions.Persistence;
using Library.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Library.Infrastructure.Persistence.Repositories;

public sealed class LoanRepository : ILoanRepository
{
    private readonly LibraryDbContext _dbContext;

    public LoanRepository(LibraryDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task AddAsync(Loan loan, CancellationToken cancellationToken)
    {
        return _dbContext.Loans.AddAsync(loan, cancellationToken).AsTask();
    }

    public async Task<Loan?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _dbContext.Loans
            .Include(loan => loan.Book)
                .ThenInclude(book => book!.Author)
            .Include(loan => loan.Reader)
            .FirstOrDefaultAsync(loan => loan.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyCollection<Loan>> GetActiveAsync(CancellationToken cancellationToken)
    {
        return await _dbContext.Loans
            .Include(loan => loan.Book)
                .ThenInclude(book => book!.Author)
            .Include(loan => loan.Reader)
            .Where(loan => loan.ReturnedAtUtc == null)
            .OrderBy(loan => loan.DueDateUtc)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<Loan>> GetByReaderIdAsync(Guid readerId, CancellationToken cancellationToken)
    {
        return await _dbContext.Loans
            .Include(loan => loan.Book)
                .ThenInclude(book => book!.Author)
            .Include(loan => loan.Reader)
            .Where(loan => loan.ReaderId == readerId)
            .OrderByDescending(loan => loan.BorrowedAtUtc)
            .ToListAsync(cancellationToken);
    }
}
