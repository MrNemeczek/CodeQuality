namespace Library.Api.Contracts.Readers;

public sealed record RegisterReaderRequest(string FirstName, string LastName, string Email);
