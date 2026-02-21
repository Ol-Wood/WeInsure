using WeInsure.Domain.Shared;
using WeInsure.Domain.ValueObjects;

namespace WeInsure.Domain.Entities;

public class Policy
{
    
    public static Result<Policy> Create(string reference, DateOnly startDate, PolicyHolder[] policyHolders, Payment payment, Money price)
    {
        return Result<Policy>.Failure(Error.Domain("Policy must have at least 1 policy holder and no more than 3."));
    }
}