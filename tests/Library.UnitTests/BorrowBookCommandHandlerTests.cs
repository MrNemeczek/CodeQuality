using FluentValidation;
using Library.Application.Abstractions.Persistence;
using Library.Application.Common.Exceptions;
using Library.Application.Loans.Commands.BorrowBook;
using Library.Domain.Entities;

namespace Library.UnitTests;

public sealed class BorrowBookCommandHandlerTests
{
    [Fact]
    public async Task HandleAsync_ShouldThrowConflict_WhenNoCopiesAreAvailable()
    {
        var author = new Author("George", "Orwell");
        var book = new Book("1984", "9780451524935", 1949, 1, author.Id);
        var reader = new Reader("Jan", "Nowak", "jan.nowak@example.com");
        var existingLoan = new Loan(book.Id, reader.Id, DateTime.UtcNow.AddDays(-1), DateTime.UtcNow.AddDays(14));
        book.Loans.Add(existingLoan);

        var handler = new BorrowBookCommandHandler(
            new TestBookRepository(book),
            new TestLoanRepository(),
            new TestReaderRepository(reader),
            new TestUnitOfWork(),
            new BorrowBookCommandValidator());

        var command = new BorrowBookCommand(book.Id, reader.Id, DateTime.UtcNow.AddDays(7));

        await Assert.ThrowsAsync<ConflictException>(() => handler.HandleAsync(command, CancellationToken.None));
    }

    private sealed class TestBookRepository : IBookRepository
    {
        private readonly Book _book;

        public TestBookRepository(Book book)
        {
            _book = book;
        }

        public Task AddAsync(Book book, CancellationToken cancellationToken)
        {
            throw new NotSupportedException();
        }

        public Task<Book?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            return Task.FromResult<Book?>(_book.Id == id ? _book : null);
        }

        public Task<IReadOnlyCollection<Book>> GetAllAsync(CancellationToken cancellationToken)
        {
            throw new NotSupportedException();
        }

        public Task<bool> IsbnExistsAsync(string isbn, Guid? excludedBookId, CancellationToken cancellationToken)
        {
            throw new NotSupportedException();
        }

        public void Remove(Book book)
        {
            throw new NotSupportedException();
        }
    }

    private sealed class TestReaderRepository : IReaderRepository
    {
        private readonly Reader _reader;

        public TestReaderRepository(Reader reader)
        {
            _reader = reader;
        }

        public Task AddAsync(Reader reader, CancellationToken cancellationToken)
        {
            throw new NotSupportedException();
        }

        public Task<Reader?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            return Task.FromResult<Reader?>(_reader.Id == id ? _reader : null);
        }

        public Task<IReadOnlyCollection<Reader>> GetAllAsync(CancellationToken cancellationToken)
        {
            throw new NotSupportedException();
        }

        public Task<bool> EmailExistsAsync(string email, Guid? excludedReaderId, CancellationToken cancellationToken)
        {
            throw new NotSupportedException();
        }
    }

    private sealed class TestLoanRepository : ILoanRepository
    {
        public Task AddAsync(Loan loan, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public Task<Loan?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            throw new NotSupportedException();
        }

        public Task<IReadOnlyCollection<Loan>> GetActiveAsync(CancellationToken cancellationToken)
        {
            throw new NotSupportedException();
        }

        public Task<IReadOnlyCollection<Loan>> GetByReaderIdAsync(Guid readerId, CancellationToken cancellationToken)
        {
            throw new NotSupportedException();
        }
    }

    private sealed class TestUnitOfWork : IUnitOfWork
    {
        public Task<int> SaveChangesAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult(0);
        }
    }
}
