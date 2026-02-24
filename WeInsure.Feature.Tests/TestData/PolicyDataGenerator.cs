using WeInsure.Domain.Entities;
using WeInsure.Domain.Enums;
using WeInsure.Domain.Shared;
using WeInsure.Domain.ValueObjects;

namespace WeInsure.Feature.Tests.TestData;

public static class PolicyDataGenerator
{
    public static Domain.Entities.Policy CreatePolicy()
    {
        var policyId = Guid.NewGuid();
        var policyReference = PolicyReference.Create();
        var moneyAmount = Money.Create(2000).OrThrow();
        var policyHolder =
            PolicyHolder.Create(Guid.NewGuid(), policyId, "John", "Doe", new DateOnly(1990, 01, 01)).OrThrow();
        var address = Address.Create("Line1", "Line2", "Line3", "M34 7ER").OrThrow();
        var insuredProperty = InsuredProperty.Create(Guid.NewGuid(), policyId, address);
        var payment = Payment.Create(Guid.NewGuid(), policyId, PaymentType.Card, moneyAmount, "REF").OrThrow();

        var policy = Domain.Entities.Policy.Create(policyId, policyReference, DateOnly.FromDateTime(DateTime.UtcNow),
            PolicyType.Household, [policyHolder], moneyAmount, insuredProperty, payment, true);

        return policy.OrThrow();
    }
    
}