using FluentValidation;
using Library.Application.Abstractions.Messaging;
using Library.Application.Abstractions.Persistence;
using Library.Application.Common.Exceptions;
using Library.Application.Common.Mappings;
using Library.Application.Loans;
using Library.Domain.Entities;

namespace Library.Application.Loans.Commands.BorrowBook;

public sealed record BorrowBookCommand(Guid BookId, Guid ReaderId, DateTime DueDateUtc) : ICommand<LoanDto>;

public sealed class BorrowBookCommandValidator : AbstractValidator<BorrowBookCommand>
{
    public BorrowBookCommandValidator()
    {
        RuleFor(x => x.BookId).NotEmpty();
        RuleFor(x => x.ReaderId).NotEmpty();
        RuleFor(x => x.DueDateUtc)
            .Must(dueDateUtc => NormalizeUtc(dueDateUtc) > DateTime.UtcNow)
            .WithMessage("Due date must be in the future.");
    }

    private static DateTime NormalizeUtc(DateTime value)
    {
        return value.Kind switch
        {
            DateTimeKind.Utc => value,
            DateTimeKind.Unspecified => DateTime.SpecifyKind(value, DateTimeKind.Utc),
            _ => value.ToUniversalTime()
        };
    }
}

public sealed class BorrowBookCommandHandler : ICommandHandler<BorrowBookCommand, LoanDto>
{
    private readonly IBookRepository _bookRepository;
    private readonly ILoanRepository _loanRepository;
    private readonly IReaderRepository _readerRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<BorrowBookCommand> _validator;

    public BorrowBookCommandHandler(
        IBookRepository bookRepository,
        ILoanRepository loanRepository,
        IReaderRepository readerRepository,
        IUnitOfWork unitOfWork,
        IValidator<BorrowBookCommand> validator)
    {
        _bookRepository = bookRepository;
        _loanRepository = loanRepository;
        _readerRepository = readerRepository;
        _unitOfWork = unitOfWork;
        _validator = validator;
    }

    public async Task<LoanDto> HandleAsync(BorrowBookCommand command, CancellationToken cancellationToken)
    {
        await _validator.ValidateAndThrowAsync(command, cancellationToken);

        var book = await _bookRepository.GetByIdAsync(command.BookId, cancellationToken)
            ?? throw new NotFoundException($"Book with id '{command.BookId}' was not found.");

        var reader = await _readerRepository.GetByIdAsync(command.ReaderId, cancellationToken)
            ?? throw new NotFoundException($"Reader with id '{command.ReaderId}' was not found.");

        var activeLoansCount = book.Loans.Count(loan => !loan.IsReturned);
        if (activeLoansCount >= book.TotalCopies)
        {
            throw new ConflictException("No copies are currently available for this book.");
        }

        var loan = new Loan(book.Id, reader.Id, DateTime.UtcNow, command.DueDateUtc);

        await _loanRepository.AddAsync(loan, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return loan.ToDto(book, reader);
    }
}
