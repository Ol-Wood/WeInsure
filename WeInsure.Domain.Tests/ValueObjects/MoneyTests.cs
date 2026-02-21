using WeInsure.Domain.Shared;
using WeInsure.Domain.ValueObjects;

namespace WeInsure.Domain.Tests.ValueObjects;

public class MoneyTests
{

    [Fact]
    public void Create_ShouldReturnDomainError_IfAmountIsNegative()
    {
        const int amount = -100;
        
        var money = Money.Create(amount);
        
        Assert.False(money.IsSuccess);
        Assert.NotNull(money.Error);
        Assert.Equal("Amount cannot be negative.", money.Error.Message);
        Assert.Equal(ErrorType.Domain, money.Error.Type);
    }
    
    
    [Fact]
    public void Create_ShouldReturnDomainError_IfAmountIsMoreThan2DecimalPlaces()
    {
        const decimal amount = 23.444m;         
        var money = Money.Create(amount);
        
        Assert.False(money.IsSuccess);
        Assert.NotNull(money.Error);
        Assert.Equal("Amount cannot have more than 2 decimal places.", money.Error.Message);
        Assert.Equal(ErrorType.Domain, money.Error.Type);
    }
}