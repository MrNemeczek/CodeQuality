using Library.Domain.Common;

namespace Library.Domain.Entities;

public sealed class Book
{
    private Book()
    {
        Title = string.Empty;
        Isbn = string.Empty;
        Loans = new List<Loan>();
    }

    public Book(string title, string isbn, int publishedYear, int totalCopies, Guid authorId)
        : this()
    {
        Id = Guid.NewGuid();
        Update(title, isbn, publishedYear, totalCopies, authorId);
    }

    public Guid Id { get; private set; }

    public string Title { get; private set; }

    public string Isbn { get; private set; }

    public int PublishedYear { get; private set; }

    public int TotalCopies { get; private set; }

    public Guid AuthorId { get; private set; }

    public Author? Author { get; private set; }

    public ICollection<Loan> Loans { get; private set; }

    public void Update(string title, string isbn, int publishedYear, int totalCopies, Guid authorId)
    {
        if (authorId == Guid.Empty)
        {
            throw new DomainRuleException("Author id cannot be empty.");
        }

        Title = Normalize(title, nameof(title));
        Isbn = NormalizeIsbn(isbn);
        PublishedYear = NormalizePublishedYear(publishedYear);
        TotalCopies = NormalizeTotalCopies(totalCopies);
        AuthorId = authorId;
    }

    private static string Normalize(string value, string paramName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new DomainRuleException($"{paramName} cannot be empty.");
        }

        return value.Trim();
    }

    private static string NormalizeIsbn(string isbn)
    {
        var normalized = Normalize(isbn, nameof(isbn));
        return normalized.ToUpperInvariant();
    }

    private static int NormalizePublishedYear(int publishedYear)
    {
        var currentYear = DateTime.UtcNow.Year + 1;

        if (publishedYear < 1450 || publishedYear > currentYear)
        {
            throw new DomainRuleException($"Published year must be between 1450 and {currentYear}.");
        }

        return publishedYear;
    }

    private static int NormalizeTotalCopies(int totalCopies)
    {
        if (totalCopies <= 0)
        {
            throw new DomainRuleException("Total copies must be greater than zero.");
        }

        return totalCopies;
    }
}
