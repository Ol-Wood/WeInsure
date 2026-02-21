using AutoFixture;
using FluentValidation;
using FluentValidation.Results;
using NSubstitute;
using WeInsure.Application.Policy.Commands;
using WeInsure.Application.Policy.UseCases;
using WeInsure.Application.Tests.AutoFixture;
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


    [Theory]
    [InlineData(61)]
    [InlineData(62)]
    [InlineData(99)]
    [InlineData(10222)]
    public async Task SellPolicy_ShouldReturnResultDomainError_IfPolicyStartDateIsMoreThan60DaysInTheFuture(int daysInFuture)
    {
        var command = new WeInsureFixture()
            .Build<SellPolicyCommand>()
            .With(x => x.StartDate, DateOnly.FromDateTime(DateTime.UtcNow.AddDays(daysInFuture)))
            .Create();
        
        var validator = Substitute.For<IValidator<SellPolicyCommand>>();
        validator.ValidateAsync(command).Returns(new ValidationResult());
        var useCase = new SellPolicyUseCase(validator);
        
        var result = await useCase.Execute(command);
        
        Assert.Null(result.Data);
        Assert.NotNull(result.Error);
        Assert.Equal(ErrorType.Domain, result.Error.Type);
        Assert.Equal("Policy start date can't be more than 60 days in the future", result.Error.Message);
    }
}