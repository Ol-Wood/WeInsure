using WeInsure.Application.Policy.Commands;
using WeInsure.Application.Policy.Dtos;
using WeInsure.Domain.Shared;

namespace WeInsure.Application.Policy.UseCases.Interfaces;

public interface IRenewPolicyUseCase
{
    Task<Result<SoldPolicy>> Execute(RenewPolicyCommand command);
}