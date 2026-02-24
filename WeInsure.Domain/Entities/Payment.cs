using WeInsure.Domain.Enums;
using WeInsure.Domain.Shared;
using WeInsure.Domain.ValueObjects;

namespace WeInsure.Domain.Entities;

public class Payment
{
    public Guid Id { get; private set; }
    public Guid PolicyId { get; private set; }
    public PaymentType PaymentType { get; private set; }
    public Money Amount { get; private set; }
    public string Reference { get; private set; }

    private Payment(){}
    private Payment(Guid id, Guid policyId, PaymentType paymentType, Money amount, string reference)
    {
        Id = id;
        PolicyId = policyId;
        PaymentType = paymentType;
        Amount = amount;
        Reference = reference;
    }
    
    public static Result<Payment> Create(Guid id, Guid policyId, PaymentType paymentType, Money amount, string reference)
    {
        if (string.IsNullOrWhiteSpace(reference))
        {
            return Result<Payment>.Failure(Error.Domain("Payment reference is required"));
        }
        
        if (!Enum.IsDefined(paymentType))
            return Result.Failure<Payment>(Error.Domain("Payment type is invalid"));
        
        return Result.Success(new Payment(id, policyId, paymentType, amount, reference));
    }

    public Payment CopyToNewPolicy(Guid newId, Guid policyId)
    {
        return Create(newId, policyId, PaymentType, Amount, Reference).OrThrow();
    }
    
}
    
    
