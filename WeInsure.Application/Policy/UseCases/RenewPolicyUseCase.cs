using WeInsure.Application.Policy.Commands;
using WeInsure.Application.Policy.Dtos;
using WeInsure.Application.Policy.UseCases.Interfaces;
using WeInsure.Application.Services;
using WeInsure.Domain.Repositories;
using WeInsure.Domain.Shared;

namespace WeInsure.Application.Policy.UseCases;

public class RenewPolicyUseCase(
    IPolicyRepository policyRepository,
    IPolicyReferenceGenerator policyReferenceGenerator,
    IIdGenerator idGenerator) : IRenewPolicyUseCase
{
    public async Task<Result<SoldPolicy>> Execute(RenewPolicyCommand command)
    {
        var policy = await policyRepository.GetByReference(command.PolicyReference);

        if (policy is null)
        {
            return Result<SoldPolicy>.Failure(Error.NotFound("Policy does not exist."));
        }

        var renewedPolicyId = idGenerator.Generate();
        var renewedPolicyReference = await policyReferenceGenerator.Generate();

        var dateOfRenewal = DateOnly.FromDateTime(DateTime.UtcNow);
        var renewedPolicy = policy.Renew(renewedPolicyId, renewedPolicyReference, dateOfRenewal);

        if (!renewedPolicy.IsSuccess)
            return Result<SoldPolicy>.Failure(renewedPolicy.Error);

        return Result<SoldPolicy>.Success(new SoldPolicy(renewedPolicy.Data.Id, renewedPolicy.Data.Reference.Value));
    }
}