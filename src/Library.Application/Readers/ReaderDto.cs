namespace Library.Application.Readers;

public sealed record ReaderDto(Guid Id, string FirstName, string LastName, string FullName, string Email);
