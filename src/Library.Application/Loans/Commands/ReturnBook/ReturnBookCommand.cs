using FluentValidation;
using Library.Application.Abstractions.Messaging;
using Library.Application.Abstractions.Persistence;
using Library.Application.Common.Exceptions;
using Library.Application.Common.Mappings;
using Library.Application.Loans;

namespace Library.Application.Loans.Commands.ReturnBook;

public sealed record ReturnBookCommand(Guid LoanId) : ICommand<LoanDto>;

public sealed class ReturnBookCommandValidator : AbstractValidator<ReturnBookCommand>
{
    public ReturnBookCommandValidator()
    {
        RuleFor(x => x.LoanId).NotEmpty();
    }
}

public sealed class ReturnBookCommandHandler : ICommandHandler<ReturnBookCommand, LoanDto>
{
    private readonly ILoanRepository _loanRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<ReturnBookCommand> _validator;

    public ReturnBookCommandHandler(
        ILoanRepository loanRepository,
        IUnitOfWork unitOfWork,
        IValidator<ReturnBookCommand> validator)
    {
        _loanRepository = loanRepository;
        _unitOfWork = unitOfWork;
        _validator = validator;
    }

    public async Task<LoanDto> HandleAsync(ReturnBookCommand command, CancellationToken cancellationToken)
    {
        await _validator.ValidateAndThrowAsync(command, cancellationToken);

        var loan = await _loanRepository.GetByIdAsync(command.LoanId, cancellationToken)
            ?? throw new NotFoundException($"Loan with id '{command.LoanId}' was not found.");

        loan.Return(DateTime.UtcNow);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return loan.ToDto();
    }
}
