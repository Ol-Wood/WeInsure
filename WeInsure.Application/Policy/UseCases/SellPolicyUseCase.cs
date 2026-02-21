using WeInsure.Application.Policy.Commands;
using WeInsure.Application.Policy.Dtos;
using WeInsure.Application.Policy.UseCases.Interfaces;
using WeInsure.Domain.Shared;

namespace WeInsure.Application.Policy.UseCases;

public class SellPolicyUseCase : ISellPolicyUseCase
{
    public Task<Result<SoldPolicy>> Execute(SellPolicyCommand command)
    {
        throw new NotImplementedException();
    }
}