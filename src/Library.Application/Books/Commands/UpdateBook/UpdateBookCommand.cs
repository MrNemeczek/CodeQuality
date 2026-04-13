using FluentValidation;
using Library.Application.Abstractions.Messaging;
using Library.Application.Abstractions.Persistence;
using Library.Application.Books;
using Library.Application.Common.Exceptions;
using Library.Application.Common.Mappings;
using Library.Domain.Entities;

namespace Library.Application.Books.Commands.UpdateBook;

public sealed record UpdateBookCommand(
    Guid BookId,
    string Title,
    string Isbn,
    int PublishedYear,
    int TotalCopies,
    string AuthorFirstName,
    string AuthorLastName) : ICommand<BookDto>;

public sealed class UpdateBookCommandValidator : AbstractValidator<UpdateBookCommand>
{
    public UpdateBookCommandValidator()
    {
        RuleFor(x => x.BookId).NotEmpty();
        RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Isbn).NotEmpty().MaximumLength(32);
        RuleFor(x => x.PublishedYear).InclusiveBetween(1450, DateTime.UtcNow.Year + 1);
        RuleFor(x => x.TotalCopies).GreaterThan(0);
        RuleFor(x => x.AuthorFirstName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.AuthorLastName).NotEmpty().MaximumLength(100);
    }
}

public sealed class UpdateBookCommandHandler : ICommandHandler<UpdateBookCommand, BookDto>
{
    private readonly IAuthorRepository _authorRepository;
    private readonly IBookRepository _bookRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<UpdateBookCommand> _validator;

    public UpdateBookCommandHandler(
        IAuthorRepository authorRepository,
        IBookRepository bookRepository,
        IUnitOfWork unitOfWork,
        IValidator<UpdateBookCommand> validator)
    {
        _authorRepository = authorRepository;
        _bookRepository = bookRepository;
        _unitOfWork = unitOfWork;
        _validator = validator;
    }

    public async Task<BookDto> HandleAsync(UpdateBookCommand command, CancellationToken cancellationToken)
    {
        await _validator.ValidateAndThrowAsync(command, cancellationToken);

        var book = await _bookRepository.GetByIdAsync(command.BookId, cancellationToken)
            ?? throw new NotFoundException($"Book with id '{command.BookId}' was not found.");

        if (await _bookRepository.IsbnExistsAsync(command.Isbn, command.BookId, cancellationToken))
        {
            throw new ConflictException($"Book with ISBN '{command.Isbn.Trim()}' already exists.");
        }

        var activeLoansCount = book.Loans.Count(loan => !loan.IsReturned);
        if (command.TotalCopies < activeLoansCount)
        {
            throw new ConflictException("Total copies cannot be lower than the number of active loans.");
        }

        var author = await _authorRepository.GetByFullNameAsync(command.AuthorFirstName, command.AuthorLastName, cancellationToken);
        if (author is null)
        {
            author = new Author(command.AuthorFirstName, command.AuthorLastName);
            await _authorRepository.AddAsync(author, cancellationToken);
        }

        book.Update(command.Title, command.Isbn, command.PublishedYear, command.TotalCopies, author.Id);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return book.ToDto(author, activeLoansCount);
    }
}
