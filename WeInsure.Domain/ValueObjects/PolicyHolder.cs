namespace WeInsure.Domain.ValueObjects;

public record PolicyHolder(string FirstName, string LastName, DateOnly DateOfBirth);