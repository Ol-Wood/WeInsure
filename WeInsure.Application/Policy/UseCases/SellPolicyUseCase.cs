using FluentValidation;
using WeInsure.Application.Policy.Commands;
using WeInsure.Application.Policy.Dtos;
using WeInsure.Application.Policy.UseCases.Interfaces;
using WeInsure.Domain.Shared;

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


        if (command.StartDate > DateOnly.FromDateTime(DateTime.UtcNow.AddDays(60)))
        {
            return Result<SoldPolicy>.Failure(Error.Domain("Policy start date can't be more than 60 days in the future")); 
        }
        
        
        
        
        throw new NotImplementedException();
    }
    
}