using WeInsure.Domain.Enums;

namespace WeInsure.Domain.ValueObjects;

public record Payment(decimal Amount, PaymentType PaymentType, string Reference);