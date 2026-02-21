using WeInsure.Domain.Enums;

namespace WeInsure.Application.Policy.Dtos;

public class PaymentDto
{
    public required string PaymentReference { get; set; }
    public decimal Amount { get; set; }
    public PaymentType PaymentType { get; set; }
}