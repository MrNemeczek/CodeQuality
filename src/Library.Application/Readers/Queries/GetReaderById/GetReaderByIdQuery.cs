using Library.Application.Abstractions.Messaging;
using Library.Application.Abstractions.Persistence;
using Library.Application.Common.Exceptions;
using Library.Application.Common.Mappings;
using Library.Application.Readers;

namespace Library.Application.Readers.Queries.GetReaderById;

public sealed record GetReaderByIdQuery(Guid ReaderId) : IQuery<ReaderDto>;

public sealed class GetReaderByIdQueryHandler : IQueryHandler<GetReaderByIdQuery, ReaderDto>
{
    private readonly IReaderRepository _readerRepository;

    public GetReaderByIdQueryHandler(IReaderRepository readerRepository)
    {
        _readerRepository = readerRepository;
    }

    public async Task<ReaderDto> HandleAsync(GetReaderByIdQuery query, CancellationToken cancellationToken)
    {
        var reader = await _readerRepository.GetByIdAsync(query.ReaderId, cancellationToken)
            ?? throw new NotFoundException($"Reader with id '{query.ReaderId}' was not found.");

        return reader.ToDto();
    }
}
