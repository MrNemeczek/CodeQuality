using Library.Api.Contracts.Readers;
using Library.Application.Abstractions.Messaging;
using Library.Application.Loans;
using Library.Application.Loans.Queries.GetReaderLoans;
using Library.Application.Readers;
using Library.Application.Readers.Commands.RegisterReader;
using Library.Application.Readers.Queries.GetReaderById;
using Library.Application.Readers.Queries.GetReaders;
using Microsoft.AspNetCore.Mvc;

namespace Library.Api.Controllers;

[ApiController]
[Route("api/readers")]
public sealed class ReadersController : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<ReaderDto>> Register(
        [FromBody] RegisterReaderRequest request,
        [FromServices] ICommandHandler<RegisterReaderCommand, ReaderDto> handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.HandleAsync(
            new RegisterReaderCommand(request.FirstName, request.LastName, request.Email),
            cancellationToken);

        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyCollection<ReaderDto>>> GetAll(
        [FromServices] IQueryHandler<GetReadersQuery, IReadOnlyCollection<ReaderDto>> handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.HandleAsync(new GetReadersQuery(), cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ReaderDto>> GetById(
        Guid id,
        [FromServices] IQueryHandler<GetReaderByIdQuery, ReaderDto> handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.HandleAsync(new GetReaderByIdQuery(id), cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id:guid}/loans")]
    public async Task<ActionResult<IReadOnlyCollection<LoanDto>>> GetLoans(
        Guid id,
        [FromServices] IQueryHandler<GetReaderLoansQuery, IReadOnlyCollection<LoanDto>> handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.HandleAsync(new GetReaderLoansQuery(id), cancellationToken);
        return Ok(result);
    }
}
