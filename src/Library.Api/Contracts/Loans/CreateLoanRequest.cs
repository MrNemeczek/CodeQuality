namespace Library.Api.Contracts.Loans;

public sealed record CreateLoanRequest
{
    public required Guid BookId { get; init; }

    public required Guid ReaderId { get; init; }

    public required DateTime DueDateUtc { get; init; }
}
