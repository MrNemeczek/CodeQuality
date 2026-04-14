using FluentValidation;
using Library.Application.Abstractions.Messaging;
using Library.Application.Abstractions.Persistence;
using Library.Application.Common.Exceptions;
using Library.Application.Common.Mappings;

namespace Library.Application.Loans.Commands.ExtendLoan;

public sealed record ExtendLoanCommand(Guid LoanId, DateOnly DueDate) : ICommand<LoanDto>;

public sealed class ExtendLoanCommandValidator : AbstractValidator<ExtendLoanCommand>
{
    public ExtendLoanCommandValidator()
    {
        RuleFor(x => x.LoanId).NotEmpty();
        RuleFor(x => x.DueDate)
            .Must(dueDate => dueDate > DateOnly.FromDateTime(DateTime.UtcNow))
            .WithMessage("Due date must be later than today.");
    }
}

public sealed class ExtendLoanCommandHandler : ICommandHandler<ExtendLoanCommand, LoanDto>
{
    private readonly ILoanRepository _loanRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<ExtendLoanCommand> _validator;

    public ExtendLoanCommandHandler(
        ILoanRepository loanRepository,
        IUnitOfWork unitOfWork,
        IValidator<ExtendLoanCommand> validator)
    {
        _loanRepository = loanRepository;
        _unitOfWork = unitOfWork;
        _validator = validator;
    }

    public async Task<LoanDto> HandleAsync(ExtendLoanCommand command, CancellationToken cancellationToken)
    {
        await _validator.ValidateAndThrowAsync(command, cancellationToken);

        var loan = await _loanRepository.GetByIdAsync(command.LoanId, cancellationToken)
            ?? throw new NotFoundException($"Loan with id '{command.LoanId}' was not found.");

        loan.Extend(ToEndOfDayUtc(command.DueDate));

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return loan.ToDto();
    }

    private static DateTime ToEndOfDayUtc(DateOnly dueDate)
    {
        return dueDate.ToDateTime(TimeOnly.MaxValue, DateTimeKind.Utc);
    }
}
