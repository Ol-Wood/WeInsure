using WeInsure.Domain.Entities;
using WeInsure.Domain.Enums;
using WeInsure.Domain.Shared;
using WeInsure.Domain.ValueObjects;

namespace WeInsure.Domain.Tests.Entities;

public class PaymentTests
{
    
    private readonly Guid _paymentId = Guid.NewGuid();
    private readonly Guid _policyId = Guid.NewGuid();
    
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Payment_Create_ShouldReturnDomainError_WhenReferenceIsNullOrWhitespace(string reference)
    {
        const PaymentType paymentType = PaymentType.Card;
        var amount = CreateMoney(100);

        var result = Payment.Create(_paymentId, _policyId, paymentType, amount, reference);

        Assert.False(result.IsSuccess);
        Assert.Null(result.Data);
        Assert.Equal(ErrorType.Domain, result.Error.Type);
        Assert.Equal("Payment reference is required", result.Error.Message);
    }

    [Fact]
    public void Payment_Create_ShouldReturnDomainError_WhenPaymentTypeIsInvalid()
    {
        const PaymentType invalidPaymentType = (PaymentType)999;
        var amount = CreateMoney(100);
        const string reference = "REF123";

        var result = Payment.Create(_paymentId, _policyId, invalidPaymentType, amount, reference);

        Assert.False(result.IsSuccess);
        Assert.Null(result.Data);
        Assert.Equal(ErrorType.Domain, result.Error.Type);
        Assert.Equal("Payment type is invalid", result.Error.Message);
    }

    [Fact]
    public void Payment_Create_ShouldReturnSuccess_WhenAllFieldsAreValid()
    {
        const PaymentType paymentType = PaymentType.Card;
        var amount = CreateMoney(100);
        const string reference = "CARD-REF-123";

        var result = Payment.Create(_paymentId, _policyId, paymentType, amount, reference);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
        Assert.Equal(_paymentId, result.Data.Id);
        Assert.Equal(_policyId, result.Data.PolicyId);
        Assert.Equal(paymentType, result.Data.PaymentType);
        Assert.Equal(amount, result.Data.Amount);
        Assert.Equal(reference, result.Data.Reference);
    }
    
    private static Money CreateMoney(decimal amount)
    {
        return Money.Create(amount).Data!;
    }
}
