using Library.Domain.Common;

namespace Library.Domain.Entities;

public sealed class Loan
{
    private Loan()
    {
    }

    public Loan(Guid bookId, Guid readerId, DateTime borrowedAtUtc, DateTime dueDateUtc)
    {
        if (bookId == Guid.Empty)
        {
            throw new DomainRuleException("Book id cannot be empty.");
        }

        if (readerId == Guid.Empty)
        {
            throw new DomainRuleException("Reader id cannot be empty.");
        }

        Id = Guid.NewGuid();
        BookId = bookId;
        ReaderId = readerId;
        BorrowedAtUtc = EnsureUtc(borrowedAtUtc);
        DueDateUtc = EnsureUtc(dueDateUtc);

        if (DueDateUtc <= BorrowedAtUtc)
        {
            throw new DomainRuleException("Due date must be later than borrow date.");
        }
    }

    public Guid Id { get; private set; }

    public Guid BookId { get; private set; }

    public Book? Book { get; private set; }

    public Guid ReaderId { get; private set; }

    public Reader? Reader { get; private set; }

    public DateTime BorrowedAtUtc { get; private set; }

    public DateTime DueDateUtc { get; private set; }

    public DateTime? ReturnedAtUtc { get; private set; }

    public bool IsReturned => ReturnedAtUtc.HasValue;

    public void Return(DateTime returnedAtUtc)
    {
        if (IsReturned)
        {
            throw new DomainRuleException("Loan has already been returned.");
        }

        var normalizedReturnedAtUtc = EnsureUtc(returnedAtUtc);

        if (normalizedReturnedAtUtc < BorrowedAtUtc)
        {
            throw new DomainRuleException("Return date cannot be earlier than borrow date.");
        }

        ReturnedAtUtc = normalizedReturnedAtUtc;
    }

    public void ExtendTo(DateTime dueDateUtc)
    {
        if (IsReturned)
        {
            throw new DomainRuleException("Returned loan cannot be extended.");
        }

        var normalizedDueDateUtc = EnsureUtc(dueDateUtc);

        if (normalizedDueDateUtc <= DueDateUtc)
        {
            throw new DomainRuleException("New due date must be later than current due date.");
        }

        DueDateUtc = normalizedDueDateUtc;
    }

    private static DateTime EnsureUtc(DateTime value)
    {
        return value.Kind switch
        {
            DateTimeKind.Utc => value,
            DateTimeKind.Unspecified => DateTime.SpecifyKind(value, DateTimeKind.Utc),
            _ => value.ToUniversalTime()
        };
    }
}
