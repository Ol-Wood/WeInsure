namespace WeInsure.Application.Policy.Dtos;

public class PolicyHolderDto
{
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public DateOnly DateOfBirth { get; set; }
}