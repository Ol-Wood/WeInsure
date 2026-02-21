using AutoFixture;
using TestUtils.AutoFixture;
using WeInsure.Domain.Entities;
using WeInsure.Domain.Enums;
using WeInsure.Domain.Shared;
using WeInsure.Domain.ValueObjects;

namespace WeInsure.Domain.Tests.Entities;

public class PolicyTests
{
    // Reqs
    // Policy can't be created more than 60 days in the future
    // Policy must have at least 1 policy holder and no more than 3
    // Policy holders must be at least 16 by start date
    // Auto renewal policies can't use cheque


    [Fact]
    public void Policy_Create_ShouldReturnDomainError_WhenThereIsNoPolicyHolder()
    {
        var startDate = DateOnly.FromDateTime(DateTime.UtcNow);
        var policyHolders = Array.Empty<PolicyHolder>();
        var payment = new Payment(CreateMoney(20), PaymentType.Card, "pay-ref");
        var policy = Policy.Create("Ref", startDate, policyHolders, payment, CreateMoney(20));
        
        Assert.False(policy.IsSuccess);
        Assert.Null(policy.Data);
        Assert.Equal(ErrorType.Domain, policy.Error.Type);
        Assert.Equal("Policy must have at least 1 policy holder and no more than 3.", policy.Error.Message);
    }
    
    [Theory]
    [InlineData(4)]
    [InlineData(5)]
    [InlineData(23)]
    public void Policy_Create_ShouldReturnDomainError_WhenThereIsMoreThan3PolicyHolders(int policyHolderCount)
    {
        var startDate = DateOnly.FromDateTime(DateTime.UtcNow);
        var policyHolders = new WeInsureFixture().CreateMany<PolicyHolder>(policyHolderCount).ToArray();
        var payment = new Payment(CreateMoney(20), PaymentType.Card, "pay-ref");
        var policy = Policy.Create("Ref", startDate, policyHolders, payment, CreateMoney(20));
        
        Assert.False(policy.IsSuccess);
        Assert.Null(policy.Data);
        Assert.Equal(ErrorType.Domain, policy.Error.Type);
        Assert.Equal("Policy must have at least 1 policy holder and no more than 3.", policy.Error.Message);
    }

    private static Money CreateMoney(decimal amount)
    {
        return Money.Create(amount).Data!;
    }
}