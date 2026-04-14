namespace Library.Api.Contracts.Readers;

public sealed record RegisterReaderRequest
{
    public required string FirstName { get; init; }

    public required string LastName { get; init; }

    public required string Email { get; init; }
}
