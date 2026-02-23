using WeInsure.Domain.Entities;
using WeInsure.Domain.Enums;
using WeInsure.Domain.Shared;
using WeInsure.Domain.ValueObjects;

namespace WeInsure.Domain.Tests.Entities;

public class PolicyTests
{
    private readonly PolicyHolder _eligiblePolicyHolder;
    private readonly Guid _policyId = Guid.CreateVersion7();


    public PolicyTests()
    {
        _eligiblePolicyHolder = CreatePolicyHolder(new DateOnly(1984, 1, 1));
    }

    [Fact]
    public void Policy_Create_ShouldReturnDomainError_WhenThereIsNoPolicyHolder()
    {
        var startDate = DateOnly.FromDateTime(DateTime.UtcNow);
        var policyHolders = Array.Empty<PolicyHolder>();
        var policy = Policy.Create(
            _policyId,
            PolicyReference.Create(),
            startDate, 
            PolicyType.Household,
            policyHolders,
            CreateMoney(20),
            CreateInsuredProperty(),
            CreatePayment(),
            true);

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
        var policyHolders = new PolicyHolder[policyHolderCount].Select(_ => CreatePolicyHolder()).ToArray();
        var policy = Policy.Create(
            _policyId,
            PolicyReference.Create(),
            startDate,
            PolicyType.Household,
            policyHolders,
            CreateMoney(20),
            CreateInsuredProperty(),
            CreatePayment(),
            true);

        Assert.False(policy.IsSuccess);
        Assert.Null(policy.Data);
        Assert.Equal(ErrorType.Domain, policy.Error.Type);
        Assert.Equal("Policy must have at least 1 policy holder and no more than 3.", policy.Error.Message);
    }


    [Fact]
    public void Policy_Create_ShouldReturnDomainError_WhenAnyPolicyHolderIsNotEligibleAge()
    {
        var startDate = DateOnly.FromDateTime(new DateTime(2000, 1, 1));
        var unEligiblePolicyHolder = PolicyHolder.Create(Guid.CreateVersion7(), _policyId, "Jane", "Doe",
            DateOnly.FromDateTime(new DateTime(1984, 1, 2))).Data!;
        var policyHolders = new[]
        {
            _eligiblePolicyHolder,
            unEligiblePolicyHolder
        };

        var policy = Policy.Create(
            _policyId,
            PolicyReference.Create(),
            startDate,
            PolicyType.Household,
            policyHolders,
            CreateMoney(20),
            CreateInsuredProperty(),
            CreatePayment(),
            true);

        Assert.False(policy.IsSuccess);
        Assert.Null(policy.Data);
        Assert.Equal(ErrorType.Domain, policy.Error.Type);
        Assert.Equal("All policy holders must be at least 16 years of age by the policy start date.",
            policy.Error.Message);
    }

    [Theory]
    [ClassData(typeof(PolicyInvalidStartDateTestData))]
    public void Policy_Create_ShouldReturnDomainError_WhenPolicyStartDateExceedsLimit(DateOnly startDate)
    {
        var policyHolders = new[]
        {
            _eligiblePolicyHolder,
        };

        var policy = Policy.Create(
            _policyId,
            PolicyReference.Create(),
            startDate,
            PolicyType.Household,
            policyHolders,
            CreateMoney(20),
            CreateInsuredProperty(),
            CreatePayment(),
            true);

        Assert.False(policy.IsSuccess);
        Assert.Null(policy.Data);
        Assert.Equal(ErrorType.Domain, policy.Error.Type);
        Assert.Equal("Policy start date can't be more than 60 days in the future", policy.Error.Message);
    }
    
    [Fact]
    public void Policy_Create_ShouldReturnDomainError_WhenPolicyTypeIsInvalid()
    {
        var policyHolders = new[]
        {
            _eligiblePolicyHolder,
        };

        var policy = Policy.Create(
            _policyId,
            PolicyReference.Create(),
            DateOnly.FromDateTime(DateTime.UtcNow.AddDays(7)),
            (PolicyType)99,
            policyHolders,
            CreateMoney(20),
            CreateInsuredProperty(),
            CreatePayment(),
            true);

        Assert.False(policy.IsSuccess);
        Assert.Null(policy.Data);
        Assert.Equal(ErrorType.Domain, policy.Error.Type);
        Assert.Equal("Policy type is not defined.", policy.Error.Message);
    }



    [Theory]
    [InlineData(PolicyType.Household)]
    [InlineData(PolicyType.BuyToLet)]
    public void Policy_Create_ShouldReturnPolicy_WhenPolicyIsValid(PolicyType policyType)
    {
        var reference = PolicyReference.Create();
        var price = CreateMoney(20);
        var insuredProperty = CreateInsuredProperty();
        var payment = CreatePayment();
        PolicyHolder[] policyHolders = [_eligiblePolicyHolder];
        var startDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(7));
        
        var policy = Policy.Create(
            _policyId,
            reference,
            startDate,
            policyType,
            policyHolders,
            price,
            insuredProperty,
            payment,
            true);

        Assert.True(policy.IsSuccess);
        Assert.Null(policy.Error);
        
        var data = policy.Data;
        Assert.Equal(reference, data.Reference);
        Assert.Equal(policyHolders, data.PolicyHolders);
        Assert.Equal(insuredProperty, data.InsuredProperty);
        Assert.Equal(policyType, data.PolicyType);
        Assert.Equal(payment, data.Payment);
        Assert.Equal(price, data.Price);
    }

    private static Money CreateMoney(decimal amount)
    {
        return Money.Create(amount).Data!;
    }

    private PolicyHolder CreatePolicyHolder(DateOnly? dateOfBirth = null)
    {
        return PolicyHolder.Create(Guid.CreateVersion7(), _policyId, "John", "Doe",
            dateOfBirth ?? new DateOnly(1990, 1, 1)).Data!;
    }

    private InsuredProperty CreateInsuredProperty()
    {
        return InsuredProperty.Create(Guid.CreateVersion7(), _policyId,
            Address.Create("123 Main Street", "New York", "USA", "12345").Data!);
    }

    private Payment CreatePayment()
    {
        return Payment.Create(Guid.CreateVersion7(), _policyId, PaymentType.Card, CreateMoney(20), "REF").Data!;
    }
}