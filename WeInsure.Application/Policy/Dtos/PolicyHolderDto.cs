namespace WeInsure.Application.Policy.Dtos;

public record PolicyHolderDto
{
    public required string FirstName { get; init; }
    public required string LastName { get; init; }
    public DateOnly DateOfBirth { get; init; }
}