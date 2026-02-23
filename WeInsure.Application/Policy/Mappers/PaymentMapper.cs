using WeInsure.Application.Policy.Dtos;
using WeInsure.Domain.Entities;

namespace WeInsure.Application.Policy.Mappers;

public static class PaymentMapper
{
    public static PaymentDto ToDto(this Payment payment)
    {
        return new PaymentDto
        {
            Amount = payment.Amount.Amount,
            PaymentType = payment.PaymentType,
            PaymentReference = payment.Reference
        };
    }
}