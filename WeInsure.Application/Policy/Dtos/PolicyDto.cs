using WeInsure.Domain.Enums;

namespace WeInsure.Application.Policy.Dtos;

public record PolicyDto
{
    public Guid Id { get; init; }
    public required string Reference { get; init; }
    public DateOnly StartDate { get; init; }
    public DateOnly EndDate { get; init; }
    public PolicyType Type { get; init; }
    public decimal Price { get; init; }
    public required IReadOnlyCollection<PolicyHolderDto> PolicyHolders { get; init; }
    public required AddressDto InsuredProperty { get; init; }
    public required PaymentDto Payment { get; init; }
}