using Library.Domain.Common;

namespace Library.Domain.Entities;

public sealed class Author
{
    private Author()
    {
        FirstName = string.Empty;
        LastName = string.Empty;
        Books = new List<Book>();
    }

    public Author(string firstName, string lastName)
        : this()
    {
        Id = Guid.NewGuid();
        UpdateName(firstName, lastName);
    }

    public Guid Id { get; private set; }

    public string FirstName { get; private set; }

    public string LastName { get; private set; }

    public string FullName => $"{FirstName} {LastName}";

    public ICollection<Book> Books { get; private set; }

    public void UpdateName(string firstName, string lastName)
    {
        FirstName = Normalize(firstName, nameof(firstName));
        LastName = Normalize(lastName, nameof(lastName));
    }

    private static string Normalize(string value, string paramName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new DomainRuleException($"{paramName} cannot be empty.");
        }

        return value.Trim();
    }
}
