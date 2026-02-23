using WeInsure.Domain.Shared;

namespace WeInsure.Domain.Entities;

public record PolicyHolder
{

    private PolicyHolder(){}
    private PolicyHolder(Guid id, Guid policyId, string firstName, string lastName, DateOnly dateOfBirth)
    {
        Id = id;
        PolicyId = policyId;
        FirstName = firstName;
        LastName = lastName;
        DateOfBirth = dateOfBirth;
    }

    public Guid Id { get; private set; }
    public Guid PolicyId { get; private set; }
    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    public DateOnly DateOfBirth { get; private set; }


    public static Result<PolicyHolder> Create(Guid id, Guid policyId, string firstName, string lastName, DateOnly dateOfBirth)
    {
        if (string.IsNullOrWhiteSpace(firstName))
        {
            return Result.Failure<PolicyHolder>(Error.Domain("Policy holder first name is required."));
        }

        if (string.IsNullOrWhiteSpace(lastName))
        {
            return Result.Failure<PolicyHolder>(Error.Domain("Policy holder last name is required."));
        }

        if (dateOfBirth > DateOnly.FromDateTime(DateTime.UtcNow))
        {
            return Result.Failure<PolicyHolder>(Error.Domain("Policy holder date of birth cannot be in the future."));
        }

        return Result.Success(new PolicyHolder(id, policyId, firstName, lastName, dateOfBirth));
    }
}