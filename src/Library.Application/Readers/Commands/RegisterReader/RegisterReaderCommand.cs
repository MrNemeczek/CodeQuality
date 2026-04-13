using FluentValidation;
using Library.Application.Abstractions.Messaging;
using Library.Application.Abstractions.Persistence;
using Library.Application.Common.Exceptions;
using Library.Application.Common.Mappings;
using Library.Application.Readers;
using Library.Domain.Entities;

namespace Library.Application.Readers.Commands.RegisterReader;

public sealed record RegisterReaderCommand(string FirstName, string LastName, string Email) : ICommand<ReaderDto>;

public sealed class RegisterReaderCommandValidator : AbstractValidator<RegisterReaderCommand>
{
    public RegisterReaderCommandValidator()
    {
        RuleFor(x => x.FirstName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.LastName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Email).NotEmpty().MaximumLength(256).EmailAddress();
    }
}

public sealed class RegisterReaderCommandHandler : ICommandHandler<RegisterReaderCommand, ReaderDto>
{
    private readonly IReaderRepository _readerRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<RegisterReaderCommand> _validator;

    public RegisterReaderCommandHandler(
        IReaderRepository readerRepository,
        IUnitOfWork unitOfWork,
        IValidator<RegisterReaderCommand> validator)
    {
        _readerRepository = readerRepository;
        _unitOfWork = unitOfWork;
        _validator = validator;
    }

    public async Task<ReaderDto> HandleAsync(RegisterReaderCommand command, CancellationToken cancellationToken)
    {
        await _validator.ValidateAndThrowAsync(command, cancellationToken);

        if (await _readerRepository.EmailExistsAsync(command.Email, null, cancellationToken))
        {
            throw new ConflictException($"Reader with email '{command.Email.Trim()}' already exists.");
        }

        var reader = new Reader(command.FirstName, command.LastName, command.Email);

        await _readerRepository.AddAsync(reader, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return reader.ToDto();
    }
}
