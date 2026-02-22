using WeInsure.Domain.Enums;
using WeInsure.Domain.Shared;
using WeInsure.Domain.ValueObjects;

namespace WeInsure.Domain.Entities;

public class Policy
{
    private const int PolicyMaxDaysInAdvance = 60;
    
    public Guid Id { get; private set; }
    public DateOnly StartDate { get; private set; }
    public DateOnly EndDate { get; private set; }
    public PolicyType PolicyType { get; private set; }
    public Money Price { get; private set; }
    public string Reference { get; private set; }
    public IReadOnlyCollection<PolicyHolder> PolicyHolders { get; private set; }
    public InsuredProperty InsuredProperty { get; private set; }
    
    public static Result<Policy> Create(
        Guid id,
        string reference, 
        DateOnly startDate, 
        PolicyHolder[] policyHolders, 
        Money price, 
        InsuredProperty insuredInsuredProperty,
        Payment payment)
    {
        if (policyHolders.Length is 0 or > 3)
        {
            return Result<Policy>.Failure(Error.Domain("Policy must have at least 1 policy holder and no more than 3."));
        }
        
        if (policyHolders.Any(ph => !IsPolicyHolderOldEnough(ph, startDate)))
        {
            return Result<Policy>.Failure(Error.Domain("All policy holders must be at least 16 years of age by the policy start date."));
        }

        if (!IsStartDateValid(startDate))
        {
            return Result<Policy>.Failure(Error.Domain("Policy start date can't be more than 60 days in the future")); 
        }
        
        throw new NotImplementedException();
    }

    private static bool IsPolicyHolderOldEnough(PolicyHolder policyHolder, DateOnly startDate)
    {
        return policyHolder.DateOfBirth.AddYears(16) <= startDate;
    }

    private static bool IsStartDateValid(DateOnly startDate)
    {
        var currentDate = DateOnly.FromDateTime(DateTime.Now);
        var maximumAdvanceStartDate = currentDate.AddDays(PolicyMaxDaysInAdvance);
        return startDate >= currentDate && startDate <= maximumAdvanceStartDate;
    }
}