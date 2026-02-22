using FluentValidation;
using WeInsure.Application.Policy.Commands;
using WeInsure.Application.Policy.Dtos;
using WeInsure.Application.Policy.UseCases.Interfaces;
using WeInsure.Domain.Entities;
using WeInsure.Domain.Shared;
using WeInsure.Domain.ValueObjects;
using PolicyEntity = WeInsure.Domain.Entities.Policy;

namespace WeInsure.Application.Policy.UseCases;

public class SellPolicyUseCase(IValidator<SellPolicyCommand> validator) : ISellPolicyUseCase
{
    public async Task<Result<SoldPolicy>> Execute(SellPolicyCommand command)
    {
        var validationResult = await validator.ValidateAsync(command);
        if (!validationResult.IsValid)
        {
            return Result<SoldPolicy>.Failure(Error.Validation("error"));
        }
        
        var policyPrice = Money.Create(command.Amount);
        if (!policyPrice.IsSuccess)
        {
            return Result<SoldPolicy>.Failure(policyPrice.Error);
        }
        
        var paidPrice = Money.Create(command.Payment.Amount);
        if (!paidPrice.IsSuccess)
        {
            return Result<SoldPolicy>.Failure(paidPrice.Error);
        }

        var policyHolders = CreatePolicyHolders(command);
        if (!policyHolders.IsSuccess)
        {
            return Result<SoldPolicy>.Failure(policyHolders.Error);
        }
        
        var policy = PolicyEntity.Create("Ref", command.StartDate, policyHolders.Data, policyPrice.Data);
        if (!policy.IsSuccess)
        {
            return Result<SoldPolicy>.Failure(policy.Error);
        }
        
        throw new NotImplementedException();
    }


    private Result<PolicyHolder[]> CreatePolicyHolders(SellPolicyCommand command)
    {
        var policyHolders = new List<PolicyHolder>();
        foreach (var holderDto in command.PolicyHolders)
        {
            var holder = PolicyHolder.Create(holderDto.FirstName, holderDto.LastName, holderDto.DateOfBirth);

            if (!holder.IsSuccess)
            {
                return Result<PolicyHolder[]>.Failure(holder.Error);
            }
           
            policyHolders.Add(holder.Data);
        }
        
        return Result<PolicyHolder[]>.Success(policyHolders.ToArray());
    }
    
}