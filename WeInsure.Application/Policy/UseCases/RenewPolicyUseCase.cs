using WeInsure.Application.Policy.Commands;
using WeInsure.Application.Policy.Dtos;
using WeInsure.Application.Policy.UseCases.Interfaces;
using WeInsure.Application.Services;
using WeInsure.Domain.Entities;
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
        
        var dateOfRenewal = DateOnly.FromDateTime(DateTime.UtcNow);
        var canRenew = policy.CanRenew(dateOfRenewal);

        if (!canRenew.IsSuccess)
        {
            return Result<SoldPolicy>.Failure(canRenew.Error);
        }
        
        var renewedPolicyId = idGenerator.Generate();
        var renewedPolicyReference = await policyReferenceGenerator.Generate();

        var renewedPolicy = Domain.Entities.Policy.Create(
            renewedPolicyId,
            renewedPolicyReference,
            policy.EndDate.AddDays(1),
            policy.PolicyType,
            ClonePolicyHolders(policy.PolicyHolders, renewedPolicyId),
            policy.Price,
            policy.InsuredProperty.CopyToNewPolicy(idGenerator.Generate(), renewedPolicyId),
            GetRenewalPayment(policy, renewedPolicyId),
            policy.AutoRenew,
            dateOfRenewal
        ).OrThrow();
        
        return Result<SoldPolicy>.Success(new SoldPolicy(renewedPolicy.Id, renewedPolicy.Reference.Value));
    }

    private Payment? GetRenewalPayment(Domain.Entities.Policy policy, Guid renewalPolicyId)
    {
        return policy.AutoRenew ? policy.Payment.CopyToNewPolicy(idGenerator.Generate(), renewalPolicyId) : null;
    }

    private PolicyHolder[] ClonePolicyHolders(IEnumerable<PolicyHolder> policyHolders, Guid newPolicyId)
    {
        return policyHolders.Select(ph => ph.CopyToNewPolicy(idGenerator.Generate(), newPolicyId)).ToArray();
    }
}