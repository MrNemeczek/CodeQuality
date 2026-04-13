using Library.Domain.Common;

namespace Library.Domain.Entities;

public sealed class Reader
{
    private Reader()
    {
        FirstName = string.Empty;
        LastName = string.Empty;
        Email = string.Empty;
        Loans = new List<Loan>();
    }

    public Reader(string firstName, string lastName, string email)
        : this()
    {
        Id = Guid.NewGuid();
        Update(firstName, lastName, email);
    }

    public Guid Id { get; private set; }

    public string FirstName { get; private set; }

    public string LastName { get; private set; }

    public string FullName => $"{FirstName} {LastName}";

    public string Email { get; private set; }

    public ICollection<Loan> Loans { get; private set; }

    public void Update(string firstName, string lastName, string email)
    {
        FirstName = Normalize(firstName, nameof(firstName));
        LastName = Normalize(lastName, nameof(lastName));
        Email = NormalizeEmail(email);
    }

    private static string Normalize(string value, string paramName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new DomainRuleException($"{paramName} cannot be empty.");
        }

        return value.Trim();
    }

    private static string NormalizeEmail(string email)
    {
        var normalized = Normalize(email, nameof(email));
        return normalized.ToLowerInvariant();
    }
}
