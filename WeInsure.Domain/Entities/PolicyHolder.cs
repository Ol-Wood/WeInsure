using WeInsure.Domain.Shared;

namespace WeInsure.Domain.Entities;

public record PolicyHolder
{

    private PolicyHolder(string firstName, string lastName, DateOnly dateOfBirth)
    {
        FirstName = firstName;
        LastName = lastName;
        DateOfBirth = dateOfBirth;
    }

    public string FirstName { get; set; }
    public string LastName { get; set; }
    public DateOnly DateOfBirth { get; set; }


    public static Result<PolicyHolder> Create(string firstName, string lastName, DateOnly dateOfBirth)
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

        return Result.Success(new PolicyHolder(firstName, lastName, dateOfBirth));
    }
}