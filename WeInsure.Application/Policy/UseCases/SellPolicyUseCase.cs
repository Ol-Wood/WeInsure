using FluentValidation;
using WeInsure.Application.Policy.Commands;
using WeInsure.Application.Policy.Dtos;
using WeInsure.Application.Policy.UseCases.Interfaces;
using WeInsure.Domain.Shared;

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
        
        if (IsPolicyStartDateValid(command))
        {
            return Result<SoldPolicy>.Failure(Error.Domain("Policy start date can't be more than 60 days in the future")); 
        }
        
        
        throw new NotImplementedException();
    }


    private static bool IsPolicyStartDateValid(SellPolicyCommand command)
    {
        var maximumAdvanceStartDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(PolicyMaxDaysInAdvance));
        return command.StartDate >= maximumAdvanceStartDate;
    }
    
}