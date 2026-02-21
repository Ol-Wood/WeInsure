using WeInsure.Domain.Shared;
using WeInsure.Domain.ValueObjects;

namespace WeInsure.Domain.Entities;

public class Policy
{
    
    public static Result<Policy> Create(string reference, DateOnly startDate, PolicyHolder[] policyHolders, Payment payment, Money amount)
    {
        return Result<Policy>.Failure(Error.Domain("There can only be a maximum of 3 policy holders"));
    }
}