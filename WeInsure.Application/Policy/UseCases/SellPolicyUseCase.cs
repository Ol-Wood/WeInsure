using FluentValidation;
using WeInsure.Application.Policy.Commands;
using WeInsure.Application.Policy.Dtos;
using WeInsure.Application.Policy.UseCases.Interfaces;
using WeInsure.Domain.Entities;
using WeInsure.Domain.Shared;
using WeInsure.Domain.ValueObjects;

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
        
        var policyHolders = command.PolicyHolders
            .Select(ph => PolicyHolder.Create(ph.FirstName, ph.LastName, ph.DateOfBirth))
            .ToArray();

        var policy = Domain.Entities.Policy.Create("Ref", command.StartDate, policyHolders, policyPrice.Data);
        if (!policy.IsSuccess)
        {
            return Result<SoldPolicy>.Failure(policy.Error);
        }
        
        throw new NotImplementedException();
    }
    
}