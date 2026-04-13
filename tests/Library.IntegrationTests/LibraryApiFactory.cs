using Library.Infrastructure.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Library.IntegrationTests;

public sealed class LibraryApiFactory : WebApplicationFactory<Program>
{
    private readonly string _databaseName = $"library-tests-{Guid.NewGuid()}";

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureServices(services =>
        {
            services.RemoveAll<DbContextOptions<LibraryDbContext>>();
            services.RemoveAll<IDbContextOptionsConfiguration<LibraryDbContext>>();
            services.RemoveAll<LibraryDbContext>();

            services.AddDbContext<LibraryDbContext>(options =>
                options.UseInMemoryDatabase(_databaseName));
        });
    }
}
