using WeInsure.Application.Policy.Dtos;
using WeInsure.Domain.Enums;

namespace WeInsure.Application.Policy.Commands;

public record SellPolicyCommand
{
    public DateOnly StartDate { get; init; }
    public PolicyType PolicyType { get; init; }
    public decimal Amount { get; init; }
    public required IReadOnlyList<PolicyHolderDto> PolicyHolders { get; init; }
    public required PaymentDto Payment { get; init; }
}