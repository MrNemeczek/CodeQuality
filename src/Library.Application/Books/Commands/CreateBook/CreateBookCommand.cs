using FluentValidation;
using Library.Application.Abstractions.Messaging;
using Library.Application.Abstractions.Persistence;
using Library.Application.Books;
using Library.Application.Common.Exceptions;
using Library.Application.Common.Mappings;
using Library.Domain.Entities;

namespace Library.Application.Books.Commands.CreateBook;

public sealed record CreateBookCommand(
    string Title,
    string Isbn,
    int PublishedYear,
    int TotalCopies,
    string AuthorFirstName,
    string AuthorLastName) : ICommand<BookDto>;

public sealed class CreateBookCommandValidator : AbstractValidator<CreateBookCommand>
{
    public CreateBookCommandValidator()
    {
        RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Isbn).NotEmpty().MaximumLength(32);
        RuleFor(x => x.PublishedYear).InclusiveBetween(1450, DateTime.UtcNow.Year + 1);
        RuleFor(x => x.TotalCopies).GreaterThan(0);
        RuleFor(x => x.AuthorFirstName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.AuthorLastName).NotEmpty().MaximumLength(100);
    }
}

public sealed class CreateBookCommandHandler : ICommandHandler<CreateBookCommand, BookDto>
{
    private readonly IAuthorRepository _authorRepository;
    private readonly IBookRepository _bookRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<CreateBookCommand> _validator;

    public CreateBookCommandHandler(
        IAuthorRepository authorRepository,
        IBookRepository bookRepository,
        IUnitOfWork unitOfWork,
        IValidator<CreateBookCommand> validator)
    {
        _authorRepository = authorRepository;
        _bookRepository = bookRepository;
        _unitOfWork = unitOfWork;
        _validator = validator;
    }

    public async Task<BookDto> HandleAsync(CreateBookCommand command, CancellationToken cancellationToken)
    {
        await _validator.ValidateAndThrowAsync(command, cancellationToken);

        if (await _bookRepository.IsbnExistsAsync(command.Isbn, null, cancellationToken))
        {
            throw new ConflictException($"Book with ISBN '{command.Isbn.Trim()}' already exists.");
        }

        var author = await _authorRepository.GetByFullNameAsync(command.AuthorFirstName, command.AuthorLastName, cancellationToken);
        if (author is null)
        {
            author = new Author(command.AuthorFirstName, command.AuthorLastName);
            await _authorRepository.AddAsync(author, cancellationToken);
        }

        var book = new Book(command.Title, command.Isbn, command.PublishedYear, command.TotalCopies, author.Id);

        await _bookRepository.AddAsync(book, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return book.ToDto(author, 0);
    }
}
