using Library.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Library.Infrastructure.Persistence;

public sealed class LibraryDataSeeder
{
    private readonly LibraryDbContext _dbContext;

    public LibraryDataSeeder(LibraryDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        if (await _dbContext.Books.AnyAsync(cancellationToken))
        {
            return;
        }

        var orwell = new Author("George", "Orwell");
        var tolkien = new Author("J.R.R.", "Tolkien");

        var nineteenEightyFour = new Book("1984", "9780451524935", 1949, 3, orwell.Id);
        var animalFarm = new Book("Animal Farm", "9780451526342", 1945, 2, orwell.Id);
        var hobbit = new Book("The Hobbit", "9780547928227", 1937, 4, tolkien.Id);

        var anna = new Reader("Anna", "Kowalska", "anna.kowalska@example.com");
        var jan = new Reader("Jan", "Nowak", "jan.nowak@example.com");

        var activeLoan = new Loan(
            nineteenEightyFour.Id,
            anna.Id,
            DateTime.UtcNow.AddDays(-4),
            DateTime.UtcNow.AddDays(10));

        var returnedLoan = new Loan(
            hobbit.Id,
            jan.Id,
            DateTime.UtcNow.AddDays(-21),
            DateTime.UtcNow.AddDays(-7));

        returnedLoan.Return(DateTime.UtcNow.AddDays(-10));

        await _dbContext.Authors.AddRangeAsync(new[] { orwell, tolkien }, cancellationToken);
        await _dbContext.Books.AddRangeAsync(new[] { nineteenEightyFour, animalFarm, hobbit }, cancellationToken);
        await _dbContext.Readers.AddRangeAsync(new[] { anna, jan }, cancellationToken);
        await _dbContext.Loans.AddRangeAsync(new[] { activeLoan, returnedLoan }, cancellationToken);

        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
