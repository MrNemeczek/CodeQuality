using Library.Api.Contracts.Loans;
using Library.Application.Abstractions.Messaging;
using Library.Application.Loans;
using Library.Application.Loans.Commands.BorrowBook;
using Library.Application.Loans.Commands.ReturnBook;
using Library.Application.Loans.Queries.GetActiveLoans;
using Microsoft.AspNetCore.Mvc;

namespace Library.Api.Controllers;

[ApiController]
[Route("api/loans")]
public sealed class LoansController : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<LoanDto>> Borrow(
        [FromBody] CreateLoanRequest request,
        [FromServices] ICommandHandler<BorrowBookCommand, LoanDto> handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.HandleAsync(
            new BorrowBookCommand(request.BookId, request.ReaderId, request.DueDateUtc),
            cancellationToken);

        return Created($"/api/loans/{result.Id}", result);
    }

    [HttpPost("{id:guid}/return")]
    public async Task<ActionResult<LoanDto>> Return(
        Guid id,
        [FromServices] ICommandHandler<ReturnBookCommand, LoanDto> handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.HandleAsync(new ReturnBookCommand(id), cancellationToken);
        return Ok(result);
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyCollection<LoanDto>>> GetActive(
        [FromServices] IQueryHandler<GetActiveLoansQuery, IReadOnlyCollection<LoanDto>> handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.HandleAsync(new GetActiveLoansQuery(), cancellationToken);
        return Ok(result);
    }
}
