using Library.Application.Abstractions.Messaging;
using Library.Application.Abstractions.Persistence;
using Library.Application.Common.Mappings;
using Library.Application.Loans;

namespace Library.Application.Loans.Queries.GetActiveLoans;

public sealed record GetActiveLoansQuery : IQuery<IReadOnlyCollection<LoanDto>>;

public sealed class GetActiveLoansQueryHandler : IQueryHandler<GetActiveLoansQuery, IReadOnlyCollection<LoanDto>>
{
    private readonly ILoanRepository _loanRepository;

    public GetActiveLoansQueryHandler(ILoanRepository loanRepository)
    {
        _loanRepository = loanRepository;
    }

    public async Task<IReadOnlyCollection<LoanDto>> HandleAsync(GetActiveLoansQuery query, CancellationToken cancellationToken)
    {
        var loans = await _loanRepository.GetActiveAsync(cancellationToken);
        return loans.Select(loan => loan.ToDto()).ToList();
    }
}
