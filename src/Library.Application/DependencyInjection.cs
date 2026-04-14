using FluentValidation;
using Library.Application.Abstractions.Messaging;
using Library.Application.Books;
using Library.Application.Books.Commands.CreateBook;
using Library.Application.Books.Commands.DeleteBook;
using Library.Application.Books.Commands.UpdateBook;
using Library.Application.Books.Queries.GetBookById;
using Library.Application.Books.Queries.GetBooks;
using Library.Application.Loans;
using Library.Application.Loans.Commands.BorrowBook;
using Library.Application.Loans.Commands.ExtendLoan;
using Library.Application.Loans.Commands.ReturnBook;
using Library.Application.Loans.Queries.GetActiveLoans;
using Library.Application.Loans.Queries.GetReaderLoans;
using Library.Application.Readers;
using Library.Application.Readers.Commands.RegisterReader;
using Library.Application.Readers.Queries.GetReaderById;
using Library.Application.Readers.Queries.GetReaders;
using Microsoft.Extensions.DependencyInjection;

namespace Library.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddValidatorsFromAssemblyContaining(typeof(DependencyInjection));

        services.AddScoped<ICommandHandler<CreateBookCommand, BookDto>, CreateBookCommandHandler>();
        services.AddScoped<ICommandHandler<UpdateBookCommand, BookDto>, UpdateBookCommandHandler>();
        services.AddScoped<ICommandHandler<DeleteBookCommand>, DeleteBookCommandHandler>();
        services.AddScoped<IQueryHandler<GetBookByIdQuery, BookDto>, GetBookByIdQueryHandler>();
        services.AddScoped<IQueryHandler<GetBooksQuery, IReadOnlyCollection<BookDto>>, GetBooksQueryHandler>();

        services.AddScoped<ICommandHandler<RegisterReaderCommand, ReaderDto>, RegisterReaderCommandHandler>();
        services.AddScoped<IQueryHandler<GetReaderByIdQuery, ReaderDto>, GetReaderByIdQueryHandler>();
        services.AddScoped<IQueryHandler<GetReadersQuery, IReadOnlyCollection<ReaderDto>>, GetReadersQueryHandler>();

        services.AddScoped<ICommandHandler<BorrowBookCommand, LoanDto>, BorrowBookCommandHandler>();
        services.AddScoped<ICommandHandler<ExtendLoanCommand, LoanDto>, ExtendLoanCommandHandler>();
        services.AddScoped<ICommandHandler<ReturnBookCommand, LoanDto>, ReturnBookCommandHandler>();
        services.AddScoped<IQueryHandler<GetActiveLoansQuery, IReadOnlyCollection<LoanDto>>, GetActiveLoansQueryHandler>();
        services.AddScoped<IQueryHandler<GetReaderLoansQuery, IReadOnlyCollection<LoanDto>>, GetReaderLoansQueryHandler>();

        return services;
    }
}
