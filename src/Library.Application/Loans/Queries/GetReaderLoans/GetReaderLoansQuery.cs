using Library.Application.Abstractions.Messaging;
using Library.Application.Abstractions.Persistence;
using Library.Application.Common.Exceptions;
using Library.Application.Common.Mappings;
using Library.Application.Loans;

namespace Library.Application.Loans.Queries.GetReaderLoans;

public sealed record GetReaderLoansQuery(Guid ReaderId) : IQuery<IReadOnlyCollection<LoanDto>>;

public sealed class GetReaderLoansQueryHandler : IQueryHandler<GetReaderLoansQuery, IReadOnlyCollection<LoanDto>>
{
    private readonly ILoanRepository _loanRepository;
    private readonly IReaderRepository _readerRepository;

    public GetReaderLoansQueryHandler(ILoanRepository loanRepository, IReaderRepository readerRepository)
    {
        _loanRepository = loanRepository;
        _readerRepository = readerRepository;
    }

    public async Task<IReadOnlyCollection<LoanDto>> HandleAsync(GetReaderLoansQuery query, CancellationToken cancellationToken)
    {
        var reader = await _readerRepository.GetByIdAsync(query.ReaderId, cancellationToken);
        if (reader is null)
        {
            throw new NotFoundException($"Reader with id '{query.ReaderId}' was not found.");
        }

        var loans = await _loanRepository.GetByReaderIdAsync(query.ReaderId, cancellationToken);
        return loans.Select(loan => loan.ToDto()).ToList();
    }
}
