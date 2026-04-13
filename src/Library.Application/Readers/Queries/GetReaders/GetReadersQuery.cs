using Library.Application.Abstractions.Messaging;
using Library.Application.Abstractions.Persistence;
using Library.Application.Common.Mappings;
using Library.Application.Readers;

namespace Library.Application.Readers.Queries.GetReaders;

public sealed record GetReadersQuery : IQuery<IReadOnlyCollection<ReaderDto>>;

public sealed class GetReadersQueryHandler : IQueryHandler<GetReadersQuery, IReadOnlyCollection<ReaderDto>>
{
    private readonly IReaderRepository _readerRepository;

    public GetReadersQueryHandler(IReaderRepository readerRepository)
    {
        _readerRepository = readerRepository;
    }

    public async Task<IReadOnlyCollection<ReaderDto>> HandleAsync(GetReadersQuery query, CancellationToken cancellationToken)
    {
        var readers = await _readerRepository.GetAllAsync(cancellationToken);
        return readers.Select(reader => reader.ToDto()).ToList();
    }
}
