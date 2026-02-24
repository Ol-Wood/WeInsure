using WeInsure.Application.Policy.Commands;
using WeInsure.Application.Policy.Dtos;
using WeInsure.Application.Policy.UseCases.Interfaces;
using WeInsure.Domain.Repositories;
using WeInsure.Domain.Shared;

namespace WeInsure.Application.Policy.UseCases;

public class RenewPolicyUseCase(IPolicyRepository policyRepository) : IRenewPolicyUseCase
{
    public async Task<Result<SoldPolicy>> Execute(RenewPolicyCommand command)
    {
        var policy = await policyRepository.GetByReference(command.PolicyReference);

        if (policy is null)
        {
            return Result<SoldPolicy>.Failure(Error.NotFound($"Policy {command.PolicyReference} does not exist."));
        }

        throw new NotImplementedException();
    }
}