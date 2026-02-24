using WeInsure.Domain.Enums;
using WeInsure.Domain.Shared;
using WeInsure.Domain.ValueObjects;
using static System.Enum;

namespace WeInsure.Domain.Entities;

public class Policy
{
    private const int PolicyMaxDaysInAdvance = 60;

    public Guid Id { get; private set; }
    public DateOnly StartDate { get; private set; }
    public DateOnly EndDate { get; private set; }
    public PolicyType PolicyType { get; private set; }
    public Money Price { get; private set; }
    public PolicyReference Reference { get; private set; }
    public IReadOnlyCollection<PolicyHolder> PolicyHolders { get; private set; }
    public InsuredProperty InsuredProperty { get; private set; }
    public Payment? Payment { get; private set; }
    public bool AutoRenew { get; private set; }
    public bool HasClaims { get; set; }
    public bool Canceled { get; private set; }

    private Policy(){}
    private Policy(
        Guid id,
        PolicyReference reference,
        DateOnly startDate,
        PolicyType policyType,
        PolicyHolder[] policyHolders,
        Money price,
        Payment? payment,
        InsuredProperty insuredProperty,
        bool autoRenew)
    {
        Id = id;
        Reference = reference;
        StartDate = startDate;
        EndDate = startDate.AddYears(1);
        PolicyType = policyType;
        Price = price;
        PolicyHolders = policyHolders.ToArray();
        Payment = payment;
        InsuredProperty = insuredProperty;
        AutoRenew = autoRenew;
    }

    public static Result<Policy> Create(
        Guid id,
        PolicyReference reference,
        DateOnly startDate,
        PolicyType policyType,
        PolicyHolder[] policyHolders,
        Money price,
        InsuredProperty insuredProperty,
        Payment? payment,
        bool autoRenew,
        DateOnly currentDate)
    {
        if (policyHolders.Length is 0 or > 3)
        {
            return Result<Policy>.Failure(
                Error.Domain("Policy must have at least 1 policy holder and no more than 3."));
        }

        if (policyHolders.Any(ph => !IsPolicyHolderOldEnough(ph, startDate)))
        {
            return Result<Policy>.Failure(
                Error.Domain("All policy holders must be at least 16 years of age by the policy start date."));
        }

        if (!IsStartDateValid(startDate, currentDate))
        {
            return Result<Policy>.Failure(Error.Domain("Policy start date can't be more than 60 days in the future"));
        }

        if (!IsDefined(policyType))
        {
            return Result<Policy>.Failure(Error.Domain("Policy type is not defined."));
        }

        return Result<Policy>.Success(new Policy(id, reference, startDate, policyType, policyHolders, price, payment,
            insuredProperty, autoRenew));
    }

    private static bool IsPolicyHolderOldEnough(PolicyHolder policyHolder, DateOnly startDate)
    {
        return policyHolder.DateOfBirth.AddYears(16) <= startDate;
    }

    private static bool IsStartDateValid(DateOnly startDate, DateOnly currentDate)
    {
        var maximumAdvanceStartDate = currentDate.AddDays(PolicyMaxDaysInAdvance);
        return startDate >= currentDate && startDate <= maximumAdvanceStartDate;
    }

    public Result CanRenew(DateOnly dateOfRenewal)
    {
        if (EndDate.AddDays(-30) > dateOfRenewal)
        {
            return Result<Policy>.Failure(Error.Domain("Too early for policy renewal"));
        }

        if (dateOfRenewal > EndDate)
        {
            return Result<Policy>.Failure(Error.Domain("Policy has expired and cannot be renewed"));
        }
        
        return Result.Success();
    }
}