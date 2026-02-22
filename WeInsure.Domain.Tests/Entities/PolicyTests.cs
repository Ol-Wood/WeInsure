using AutoFixture;
using TestUtils.AutoFixture;
using WeInsure.Domain.Entities;
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

    private readonly PolicyHolder _eligiblePolicyHolder = 
        new("John", "Doe", DateOnly.FromDateTime(new DateTime(1984, 1, 1)));


    [Fact]
    public void Policy_Create_ShouldReturnDomainError_WhenThereIsNoPolicyHolder()
    {
        var startDate = DateOnly.FromDateTime(DateTime.UtcNow);
        var policyHolders = Array.Empty<PolicyHolder>();
        var policy = Policy.Create("Ref", startDate, policyHolders, CreateMoney(20));
        
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
        var policy = Policy.Create("Ref", startDate, policyHolders, CreateMoney(20));
        
        Assert.False(policy.IsSuccess);
        Assert.Null(policy.Data);
        Assert.Equal(ErrorType.Domain, policy.Error.Type);
        Assert.Equal("Policy must have at least 1 policy holder and no more than 3.", policy.Error.Message);
    }


    [Fact]
    public void Policy_Create_ShouldReturnDomainError_WhenAnyPolicyHolderIsNotEligibleAge()
    {
        var startDate = DateOnly.FromDateTime(new DateTime(2000, 1, 1));
        var unEligiblePolicyHolder = new PolicyHolder("Jane", "Doe", DateOnly.FromDateTime(new DateTime(1984, 1, 2)));
        var policyHolders = new[]
        {
            _eligiblePolicyHolder,
           unEligiblePolicyHolder
        };
          
        var policy = Policy.Create("Ref", startDate, policyHolders, CreateMoney(20));
        
        Assert.False(policy.IsSuccess);
        Assert.Null(policy.Data);
        Assert.Equal(ErrorType.Domain, policy.Error.Type);
        Assert.Equal("All policy holders must be at least 16 years of age by the policy start date.", policy.Error.Message);
    }

    [Theory]
    [ClassData(typeof(PolicyInvalidStartDateTestData))]
    public void Policy_Create_ShouldReturnDomainError_WhenPolicyStartDateExceedsLimit(DateOnly startDate)
    {
        var policyHolders = new[]
        {
            _eligiblePolicyHolder,
        };
          
        var policy = Policy.Create("Ref", startDate, policyHolders, CreateMoney(20));
        
        Assert.False(policy.IsSuccess);
        Assert.Null(policy.Data);
        Assert.Equal(ErrorType.Domain, policy.Error.Type);
        Assert.Equal("Policy start date can't be more than 60 days in the future", policy.Error.Message);

    }

    private static Money CreateMoney(decimal amount)
    {
        return Money.Create(amount).Data!;
    }
}