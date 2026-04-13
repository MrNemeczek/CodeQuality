using System.Net;
using System.Net.Http.Json;

namespace Library.IntegrationTests;

public sealed class LibraryApiTests : IClassFixture<LibraryApiFactory>
{
    private readonly HttpClient _httpClient;

    public LibraryApiTests(LibraryApiFactory factory)
    {
        _httpClient = factory.CreateClient();
    }

    [Fact]
    public async Task BorrowAndReturnFlow_ShouldWorkEndToEnd()
    {
        var createBookResponse = await _httpClient.PostAsJsonAsync(
            "/api/books",
            new
            {
                title = "Refactoring",
                isbn = $"978013475759{Random.Shared.Next(0, 9)}",
                publishedYear = 2019,
                totalCopies = 2,
                authorFirstName = "Martin",
                authorLastName = "Fowler"
            });

        Assert.Equal(HttpStatusCode.Created, createBookResponse.StatusCode);
        var createdBook = await createBookResponse.Content.ReadFromJsonAsync<BookResponse>();
        Assert.NotNull(createdBook);

        var getBookResponse = await _httpClient.GetAsync($"/api/books/{createdBook!.Id}");
        Assert.Equal(HttpStatusCode.OK, getBookResponse.StatusCode);

        var createReaderResponse = await _httpClient.PostAsJsonAsync(
            "/api/readers",
            new
            {
                firstName = "Maria",
                lastName = "Wisniewska",
                email = $"maria.{Guid.NewGuid():N}@example.com"
            });

        Assert.Equal(HttpStatusCode.Created, createReaderResponse.StatusCode);
        var createdReader = await createReaderResponse.Content.ReadFromJsonAsync<ReaderResponse>();
        Assert.NotNull(createdReader);

        var getReaderResponse = await _httpClient.GetAsync($"/api/readers/{createdReader!.Id}");
        Assert.Equal(HttpStatusCode.OK, getReaderResponse.StatusCode);

        var createLoanResponse = await _httpClient.PostAsJsonAsync(
            "/api/loans",
            new
            {
                bookId = createdBook!.Id,
                readerId = createdReader!.Id,
                dueDateUtc = DateTime.UtcNow.AddDays(14)
            });

        if (createLoanResponse.StatusCode != HttpStatusCode.Created)
        {
            var errorContent = await createLoanResponse.Content.ReadAsStringAsync();
            Assert.Fail(errorContent);
        }

        var createdLoan = await createLoanResponse.Content.ReadFromJsonAsync<LoanResponse>();
        Assert.NotNull(createdLoan);
        Assert.False(createdLoan!.IsReturned);

        var returnLoanResponse = await _httpClient.PostAsync($"/api/loans/{createdLoan.Id}/return", null);

        Assert.Equal(HttpStatusCode.OK, returnLoanResponse.StatusCode);
        var returnedLoan = await returnLoanResponse.Content.ReadFromJsonAsync<LoanResponse>();
        Assert.NotNull(returnedLoan);
        Assert.True(returnedLoan!.IsReturned);

        var readerLoans = await _httpClient.GetFromJsonAsync<List<LoanResponse>>($"/api/readers/{createdReader.Id}/loans");

        Assert.NotNull(readerLoans);
        Assert.Single(readerLoans!);
        Assert.True(readerLoans[0].IsReturned);
    }

    private sealed record BookResponse(Guid Id);

    private sealed record ReaderResponse(Guid Id);

    private sealed record LoanResponse(Guid Id, bool IsReturned);
}
