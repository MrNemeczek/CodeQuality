using Library.Api.Contracts.Books;
using Library.Application.Abstractions.Messaging;
using Library.Application.Books;
using Library.Application.Books.Commands.CreateBook;
using Library.Application.Books.Commands.DeleteBook;
using Library.Application.Books.Commands.UpdateBook;
using Library.Application.Books.Queries.GetBookById;
using Library.Application.Books.Queries.GetBooks;
using Microsoft.AspNetCore.Mvc;

namespace Library.Api.Controllers;

[ApiController]
[Route("api/books")]
public sealed class BooksController : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<BookDto>> Create(
        [FromBody] CreateBookRequest request,
        [FromServices] ICommandHandler<CreateBookCommand, BookDto> handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.HandleAsync(
            new CreateBookCommand(
                request.Title,
                request.Isbn,
                request.PublishedYear,
                request.TotalCopies,
                request.AuthorFirstName,
                request.AuthorLastName),
            cancellationToken);

        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyCollection<BookDto>>> GetAll(
        [FromServices] IQueryHandler<GetBooksQuery, IReadOnlyCollection<BookDto>> handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.HandleAsync(new GetBooksQuery(), cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<BookDto>> GetById(
        Guid id,
        [FromServices] IQueryHandler<GetBookByIdQuery, BookDto> handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.HandleAsync(new GetBookByIdQuery(id), cancellationToken);
        return Ok(result);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<BookDto>> Update(
        Guid id,
        [FromBody] UpdateBookRequest request,
        [FromServices] ICommandHandler<UpdateBookCommand, BookDto> handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.HandleAsync(
            new UpdateBookCommand(
                id,
                request.Title,
                request.Isbn,
                request.PublishedYear,
                request.TotalCopies,
                request.AuthorFirstName,
                request.AuthorLastName),
            cancellationToken);

        return Ok(result);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(
        Guid id,
        [FromServices] ICommandHandler<DeleteBookCommand> handler,
        CancellationToken cancellationToken)
    {
        await handler.HandleAsync(new DeleteBookCommand(id), cancellationToken);
        return NoContent();
    }
}
