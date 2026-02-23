using WeInsure.Application.Policy.Dtos;
using WeInsure.Application.Policy.UseCases.Interfaces;
using WeInsure.Domain.Repositories;

namespace WeInsure.Application.Policy.UseCases;

public class GetPolicyUseCase(IPolicyRepository policyRepository) : IGetPolicyUseCase
{
    public Task<PolicyDto?> Execute(string reference)
    {
        throw new NotImplementedException();
    }
}