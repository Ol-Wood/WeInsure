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


    public static PolicyHolder Create(string firstName, string lastName, DateOnly dateOfBirth)
    {
        return new PolicyHolder(firstName, lastName, dateOfBirth);
    }
}