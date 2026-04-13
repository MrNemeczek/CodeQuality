using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Library.Infrastructure.Persistence;

public sealed class LibraryDbContextFactory : IDesignTimeDbContextFactory<LibraryDbContext>
{
    public LibraryDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<LibraryDbContext>();
        var connectionString = Environment.GetEnvironmentVariable("LIBRARY_CONNECTION_STRING")
            ?? "Host=localhost;Port=5432;Database=library_db;Username=postgres;Password=postgres";

        optionsBuilder.UseNpgsql(connectionString);

        return new LibraryDbContext(optionsBuilder.Options);
    }
}
