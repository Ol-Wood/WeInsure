using WeInsure.Application.Policy.Dtos;

namespace WeInsure.Application.Policy.UseCases.Interfaces;

public interface IGetPolicyUseCase
{
    Task<PolicyDto?> Execute(string reference);
}