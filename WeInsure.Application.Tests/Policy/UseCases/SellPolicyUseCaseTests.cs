using AutoFixture;
using FluentValidation;
using FluentValidation.Results;
using NSubstitute;
using TestUtils.AutoFixture;
using WeInsure.Application.Policy.Commands;
using WeInsure.Application.Policy.Dtos;
using WeInsure.Application.Policy.UseCases;
using WeInsure.Domain.Shared;

namespace WeInsure.Application.Tests.Policy.UseCases;

public class SellPolicyUseCaseTests
{

    [Fact]
    public async Task SellPolicy_ShouldReturnResultValidationError_IfCommandValidationFails()
    {
        var command = new WeInsureFixture().Create<SellPolicyCommand>();
        var validator = Substitute.For<IValidator<SellPolicyCommand>>();
        validator.ValidateAsync(command).Returns(new ValidationResult([new ValidationFailure("", "")]));
        var useCase = new SellPolicyUseCase(validator);
        
        var result = await useCase.Execute(command);
        
        Assert.Null(result.Data);
        Assert.NotNull(result.Error);
        Assert.Equal(ErrorType.Validation, result.Error.Type);
    }
    
    [Fact]
    public async Task SellPolicy_ShouldReturnDomainError_IfPolicyPriceAmountIsNotValid()
    {
        var command = new WeInsureFixture()
            .Build<SellPolicyCommand>()
            .With(x => x.StartDate, DateOnly.FromDateTime(DateTime.UtcNow.AddDays(10)))
            .With(x =>x.PolicyHolders, [])
            .With(x => x.Amount, 2.789m)
            .Create();
        
        var validator = Substitute.For<IValidator<SellPolicyCommand>>();
        validator.ValidateAsync(command).Returns(new ValidationResult());
        var useCase = new SellPolicyUseCase(validator);
        
        var result = await useCase.Execute(command);
        
        Assert.Null(result.Data);
        Assert.NotNull(result.Error);
        Assert.Equal(ErrorType.Domain, result.Error.Type);
        Assert.Equal("Amount cannot have more than 2 decimal places.", result.Error.Message);
    }


    [Fact]
    public async Task SellPolicy_ShouldReturnResultDomainError_IfPolicyCreationCausesDomainError()
    {
        var tooManyPolicyHolders = new WeInsureFixture().CreateMany<PolicyHolderDto>(4);
        var command = new WeInsureFixture()
            .Build<SellPolicyCommand>()
            .With(x => x.StartDate, DateOnly.FromDateTime(DateTime.UtcNow.AddDays(10)))
            .With(x =>x.PolicyHolders, tooManyPolicyHolders.ToList())
            .Create();
        
        var validator = Substitute.For<IValidator<SellPolicyCommand>>();
        validator.ValidateAsync(command).Returns(new ValidationResult());
        var useCase = new SellPolicyUseCase(validator);
        
        var result = await useCase.Execute(command);
        
        Assert.Null(result.Data);
        Assert.NotNull(result.Error);
        Assert.Equal(ErrorType.Domain, result.Error.Type);
        Assert.Equal("Policy must have at least 1 policy holder and no more than 3.", result.Error.Message);
    }

}