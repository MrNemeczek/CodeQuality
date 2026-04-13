using Library.Domain.Common;
using Library.Domain.Entities;

namespace Library.UnitTests;

public sealed class LoanTests
{
    [Fact]
    public void Return_ShouldSetReturnedAtUtc_WhenLoanIsActive()
    {
        var loan = new Loan(Guid.NewGuid(), Guid.NewGuid(), DateTime.UtcNow.AddDays(-3), DateTime.UtcNow.AddDays(7));
        var returnedAtUtc = DateTime.UtcNow;

        loan.Return(returnedAtUtc);

        Assert.True(loan.IsReturned);
        Assert.NotNull(loan.ReturnedAtUtc);
    }

    [Fact]
    public void Return_ShouldThrow_WhenLoanWasAlreadyReturned()
    {
        var loan = new Loan(Guid.NewGuid(), Guid.NewGuid(), DateTime.UtcNow.AddDays(-3), DateTime.UtcNow.AddDays(7));
        loan.Return(DateTime.UtcNow);

        Assert.Throws<DomainRuleException>(() => loan.Return(DateTime.UtcNow.AddMinutes(1)));
    }
}
