using FluentValidation;
using Library.Application.Abstractions.Messaging;
using Library.Application.Abstractions.Persistence;
using Library.Application.Common.Exceptions;

namespace Library.Application.Books.Commands.DeleteBook;

public sealed record DeleteBookCommand(Guid BookId) : ICommand;

public sealed class DeleteBookCommandValidator : AbstractValidator<DeleteBookCommand>
{
    public DeleteBookCommandValidator()
    {
        RuleFor(x => x.BookId).NotEmpty();
    }
}

public sealed class DeleteBookCommandHandler : ICommandHandler<DeleteBookCommand>
{
    private readonly IBookRepository _bookRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<DeleteBookCommand> _validator;

    public DeleteBookCommandHandler(
        IBookRepository bookRepository,
        IUnitOfWork unitOfWork,
        IValidator<DeleteBookCommand> validator)
    {
        _bookRepository = bookRepository;
        _unitOfWork = unitOfWork;
        _validator = validator;
    }

    public async Task HandleAsync(DeleteBookCommand command, CancellationToken cancellationToken)
    {
        await _validator.ValidateAndThrowAsync(command, cancellationToken);

        var book = await _bookRepository.GetByIdAsync(command.BookId, cancellationToken)
            ?? throw new NotFoundException($"Book with id '{command.BookId}' was not found.");

        if (book.Loans.Any(loan => !loan.IsReturned))
        {
            throw new ConflictException("Cannot delete a book with active loans.");
        }

        _bookRepository.Remove(book);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
