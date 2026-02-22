using WeInsure.Domain.Enums;

namespace WeInsure.Application.Policy.Dtos;

public record PaymentDto
{
    public required string PaymentReference { get; init; }
    public decimal Amount { get; init; }
    public PaymentType PaymentType { get; init; }
}