using WeInsure.Application.Policy.Commands;
using WeInsure.Application.Policy.Dtos;
using WeInsure.Application.Policy.UseCases.Interfaces;
using WeInsure.Domain.Shared;

namespace WeInsure.Application.Policy.UseCases;

public class RenewPolicyUseCase : IRenewPolicyUseCase
{
    public Task<Result<SoldPolicy>> Execute(RenewPolicyCommand command)
    {
        throw new NotImplementedException();
    }
}