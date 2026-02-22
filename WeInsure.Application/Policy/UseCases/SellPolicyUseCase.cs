using FluentValidation;
using WeInsure.Application.Policy.Commands;
using WeInsure.Application.Policy.Dtos;
using WeInsure.Application.Policy.UseCases.Interfaces;
using WeInsure.Application.Services;
using WeInsure.Application.Utils;
using WeInsure.Domain.Entities;
using WeInsure.Domain.Repositories;
using WeInsure.Domain.Shared;
using WeInsure.Domain.ValueObjects;
using PolicyEntity = WeInsure.Domain.Entities.Policy;

namespace WeInsure.Application.Policy.UseCases;

public class SellPolicyUseCase(
    IValidator<SellPolicyCommand> validator,
    IIdGenerator idGenerator,
    IPolicyRepository policyRepository,
    IPolicyReferenceGenerator policyReferenceGenerator) : ISellPolicyUseCase
{
    public async Task<Result<SoldPolicy>> Execute(SellPolicyCommand command)
    {
        var validationResult = await validator.ValidateAsync(command);
        if (!validationResult.IsValid)
        {
            return Result<SoldPolicy>.Failure(validationResult.ToValidationError());
        }

        var policyId = idGenerator.Generate();
        var policyPrice = Money.Create(command.Amount).OrThrow();
        var paidPrice = Money.Create(command.Payment.Amount).OrThrow();

        var addressDto = command.PolicyAddress;
        var address = Address.Create(
                addressDto.AddressLine1,
                addressDto.AddressLine2,
                addressDto.AddressLine3,
                addressDto.PostCode)
            .OrThrow();

        var property = InsuredProperty.Create(idGenerator.Generate(), policyId, address);

        var policyHolders = CreatePolicyHolders(command, policyId);
        if (!policyHolders.IsSuccess)
        {
            return Result<SoldPolicy>.Failure(policyHolders.Error);
        }

        var payment = Payment.Create(
            idGenerator.Generate(),
            policyId,
            command.Payment.PaymentType,
            paidPrice,
            command.Payment.PaymentReference);
        if (!payment.IsSuccess)
        {
            return Result<SoldPolicy>.Failure(payment.Error);
        }

        var policyReference = await policyReferenceGenerator.Generate();
        var policy = PolicyEntity.Create(
            policyId,
            policyReference,
            command.StartDate,
            command.PolicyType,
            policyHolders.Data,
            policyPrice,
            property,
            payment.Data);
        if (!policy.IsSuccess)
        {
            return Result<SoldPolicy>.Failure(policy.Error);
        }

        await policyRepository.Add(policy.Data);

        return Result.Success(new SoldPolicy(policyId, policyReference.Value));
    }


    private Result<PolicyHolder[]> CreatePolicyHolders(SellPolicyCommand command, Guid policyId)
    {
        var policyHolders = new List<PolicyHolder>();
        foreach (var holderDto in command.PolicyHolders)
        {
            var holder = PolicyHolder.Create(
                idGenerator.Generate(),
                policyId,
                holderDto.FirstName,
                holderDto.LastName,
                holderDto.DateOfBirth);

            if (!holder.IsSuccess)
            {
                return Result<PolicyHolder[]>.Failure(holder.Error);
            }

            policyHolders.Add(holder.Data);
        }

        return Result<PolicyHolder[]>.Success(policyHolders.ToArray());
    }
}