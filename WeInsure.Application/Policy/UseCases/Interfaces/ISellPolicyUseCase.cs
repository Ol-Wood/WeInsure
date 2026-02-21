using WeInsure.Application.Policy.Commands;

namespace WeInsure.Application.Policy.UseCases.Interfaces;

public interface ISellPolicyUseCase
{
    Task Execute(SellPolicyCommand command);
}