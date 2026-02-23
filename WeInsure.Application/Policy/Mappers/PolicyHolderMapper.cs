using WeInsure.Application.Policy.Dtos;
using WeInsure.Domain.Entities;

namespace WeInsure.Application.Policy.Mappers;

public static class PolicyHolderMapper
{
    public static PolicyHolderDto ToDto(this PolicyHolder policyHolder)
    {
        return new PolicyHolderDto
        {
            FirstName = policyHolder.FirstName,
            LastName = policyHolder.LastName,
            DateOfBirth = policyHolder.DateOfBirth,
        };
    }
}