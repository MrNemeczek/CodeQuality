namespace Library.Api.Contracts.Loans;

public sealed record CreateLoanRequest(Guid BookId, Guid ReaderId, DateTime DueDateUtc);
