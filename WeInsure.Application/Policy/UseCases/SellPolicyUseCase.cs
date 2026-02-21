using WeInsure.Application.Policy.Commands;
using WeInsure.Application.Policy.Dtos;
using WeInsure.Application.Policy.UseCases.Interfaces;

namespace WeInsure.Application.Policy.UseCases;

public class SellPolicyUseCase : ISellPolicyUseCase
{
    public Task<SellPolicyResultDto> Execute(SellPolicyCommand command)
    {
        throw new NotImplementedException();
    }
}