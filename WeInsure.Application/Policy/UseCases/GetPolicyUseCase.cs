using WeInsure.Application.Policy.Dtos;
using WeInsure.Application.Policy.UseCases.Interfaces;

namespace WeInsure.Application.Policy.UseCases;

public class GetPolicyUseCase : IGetPolicyUseCase
{
    public Task<PolicyDto?> Execute(string reference)
    {
        throw new NotImplementedException();
    }
}