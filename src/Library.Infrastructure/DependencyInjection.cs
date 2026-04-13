using Library.Application.Abstractions.Persistence;
using Library.Infrastructure.Persistence;
using Library.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Library.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("LibraryDatabase")
            ?? throw new InvalidOperationException("Connection string 'LibraryDatabase' was not found.");

        services.AddDbContext<LibraryDbContext>(options => options.UseNpgsql(connectionString));

        services.AddScoped<IAuthorRepository, AuthorRepository>();
        services.AddScoped<IBookRepository, BookRepository>();
        services.AddScoped<IReaderRepository, ReaderRepository>();
        services.AddScoped<ILoanRepository, LoanRepository>();
        services.AddScoped<IUnitOfWork>(provider => provider.GetRequiredService<LibraryDbContext>());
        services.AddScoped<LibraryDataSeeder>();

        return services;
    }
}
