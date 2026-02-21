using WeInsure.Domain.Shared;
using WeInsure.Domain.ValueObjects;

namespace WeInsure.Domain.Entities;

public class Policy
{
    
    public static Result<Policy> Create(string reference, DateOnly startDate, PolicyHolder[] policyHolders, Payment payment, Money price)
    {
        if (policyHolders.Length is 0 or > 3)
        {
            return Result<Policy>.Failure(Error.Domain("Policy must have at least 1 policy holder and no more than 3."));
        }


        if (policyHolders.Any(ph => ph.DateOfBirth.AddYears(16) > startDate))
        {
            return Result<Policy>.Failure(Error.Domain("All policy holders must be at least 16 years of age by the policy start date."));
        }


        throw new NotImplementedException();
    }
}