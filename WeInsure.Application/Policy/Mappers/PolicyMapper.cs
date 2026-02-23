using WeInsure.Application.Policy.Dtos;

namespace WeInsure.Application.Policy.Mappers;

public static class PolicyMapper
{
    public static PolicyDto ToDto(this Domain.Entities.Policy policy)
    {
        return new PolicyDto
        {
            Id = policy.Id,
            Reference = policy.Reference.Value,
            StartDate = policy.StartDate,
            EndDate = policy.EndDate,
            Type = policy.PolicyType,
            Price = policy.Price.Amount,
            PolicyHolders = policy.PolicyHolders.Select(PolicyHolderMapper.ToDto).ToArray(),
            InsuredProperty = policy.InsuredProperty.Address.ToDto(),
            Payment = policy.Payment.ToDto(),
        };
    }
}