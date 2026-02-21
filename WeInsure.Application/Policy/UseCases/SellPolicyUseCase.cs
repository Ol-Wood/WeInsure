using FluentValidation;
using WeInsure.Application.Policy.Commands;
using WeInsure.Application.Policy.Dtos;
using WeInsure.Application.Policy.UseCases.Interfaces;
using WeInsure.Domain.Shared;
using WeInsure.Domain.ValueObjects;

namespace WeInsure.Application.Policy.UseCases;

public class SellPolicyUseCase(IValidator<SellPolicyCommand> validator) : ISellPolicyUseCase
{
    private const int PolicyMaxDaysInAdvance = 60;
    
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
        
        if (IsPolicyStartDateTooFarInAdvance(command))
        {
            return Result<SoldPolicy>.Failure(Error.Domain("Policy start date can't be more than 60 days in the future")); 
        }
        
        var policyHolders = command.PolicyHolders
            .Select(ph => new PolicyHolder(ph.FirstName, ph.LastName, ph.DateOfBirth))
            .ToArray();

        var payment = new Payment(
            paidPrice.Data, 
            command.Payment.PaymentType,
            command.Payment.PaymentReference);

        var policy = Domain.Entities.Policy.Create("Ref", command.StartDate, policyHolders, payment, policyPrice.Data);
        if (!policy.IsSuccess)
        {
            return Result<SoldPolicy>.Failure(policy.Error);
        }
        
        throw new NotImplementedException();
    }


    private static bool IsPolicyStartDateTooFarInAdvance(SellPolicyCommand command)
    {
        var maximumAdvanceStartDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(PolicyMaxDaysInAdvance));
        return command.StartDate >= maximumAdvanceStartDate;
    }
    
}