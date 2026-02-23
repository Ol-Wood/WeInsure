using WeInsure.Application.Policy.Dtos;
using WeInsure.Application.Policy.Mappers;
using WeInsure.Application.Policy.UseCases.Interfaces;
using WeInsure.Domain.Repositories;

namespace WeInsure.Application.Policy.UseCases;

public class GetPolicyUseCase(IPolicyRepository policyRepository) : IGetPolicyUseCase
{
    public async Task<PolicyDto?> Execute(string reference)
    {
       var policy = await policyRepository.GetByReference(reference);

       return policy?.ToDto();
    }
}