using WeInsure.Domain.Enums;

namespace WeInsure.Domain.ValueObjects;

public record Payment(Money Amount, PaymentType PaymentType, string Reference);