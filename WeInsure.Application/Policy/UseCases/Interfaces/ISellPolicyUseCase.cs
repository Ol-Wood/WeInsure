using WeInsure.Application.Policy.Commands;
using WeInsure.Application.Policy.Dtos;

namespace WeInsure.Application.Policy.UseCases.Interfaces;

public interface ISellPolicyUseCase
{
    Task<SellPolicyResultDto> Execute(SellPolicyCommand command);
}