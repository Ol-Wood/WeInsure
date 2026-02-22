using WeInsure.Domain.Entities;
using WeInsure.Domain.Enums;
using WeInsure.Domain.Shared;
using WeInsure.Domain.ValueObjects;

namespace WeInsure.Domain.Tests.Entities;

public class PaymentTests
{

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Payment_Create_ShouldReturnDomainError_WhenReferenceIsNullOrWhitespace(string reference)
    {
        var id = Guid.NewGuid();
        const PaymentType paymentType = PaymentType.Card;
        var amount = CreateMoney(100);

        var result = Payment.Create(id, paymentType, amount, reference);

        Assert.False(result.IsSuccess);
        Assert.Null(result.Data);
        Assert.Equal(ErrorType.Domain, result.Error.Type);
        Assert.Equal("Payment reference is required", result.Error.Message);
    }

    [Fact]
    public void Payment_Create_ShouldReturnDomainError_WhenPaymentTypeIsInvalid()
    {
        var id = Guid.NewGuid();
        const PaymentType invalidPaymentType = (PaymentType)999;
        var amount = CreateMoney(100);
        const string reference = "REF123";

        var result = Payment.Create(id, invalidPaymentType, amount, reference);

        Assert.False(result.IsSuccess);
        Assert.Null(result.Data);
        Assert.Equal(ErrorType.Domain, result.Error.Type);
        Assert.Equal("Payment type is invalid", result.Error.Message);
    }

    [Fact]
    public void Payment_Create_ShouldReturnSuccess_WhenAllFieldsAreValid()
    {
        var id = Guid.NewGuid();
        const PaymentType paymentType = PaymentType.Card;
        var amount = CreateMoney(100);
        const string reference = "CARD-REF-123";

        var result = Payment.Create(id, paymentType, amount, reference);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
        Assert.Equal(id, result.Data.Id);
        Assert.Equal(paymentType, result.Data.PaymentType);
        Assert.Equal(amount, result.Data.Amount);
        Assert.Equal(reference, result.Data.Reference);
    }
    
    private static Money CreateMoney(decimal amount)
    {
        return Money.Create(amount).Data!;
    }
}
