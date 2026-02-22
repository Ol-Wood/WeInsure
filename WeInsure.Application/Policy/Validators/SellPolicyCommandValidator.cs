using FluentValidation;
using WeInsure.Application.Policy.Commands;
using WeInsure.Application.Utils;
using WeInsure.Domain.ValueObjects;

namespace WeInsure.Application.Policy.Validators;

public class SellPolicyCommandValidator : AbstractValidator<SellPolicyCommand>
{
    public SellPolicyCommandValidator()
    {
        RuleFor(x => x.StartDate)
            .NotEqual(DateOnly.MinValue);
        
        RuleFor(x => x.Amount)
            .MustBeSuccessfulResult(Money.Create);
        
        RuleFor(x => x.Payment.Amount)
            .MustBeSuccessfulResult(Money.Create);
        
        RuleFor(x => x.PolicyAddress)
            .MustBeSuccessfulResult(x => Address.Create(x.AddressLine1, x.AddressLine2, x.AddressLine3, x.PostCode));
        
        RuleFor(x => x.PolicyType)
            .Must(Enum.IsDefined);
        
        RuleFor(x => x.PolicyHolders)
            .NotEmpty();
    }
}