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

    private readonly IValidator<SellPolicyCommand> _validator = Substitute.For<IValidator<SellPolicyCommand>>();
    private readonly SellPolicyUseCase _useCase;

    private readonly AddressDto _validAddressDto = new()
    {
        AddressLine1 = "123 Main Street",
        AddressLine2 = "Some other street",
        AddressLine3 = "Some other street",
        PostCode = "M21 8HG"
    };

    private readonly PolicyHolderDto _validPolicyHolder = new()
    {
        FirstName = "John",
        LastName = "Doe",
        DateOfBirth = new DateOnly(1990, 1, 1),
    };
    
    public SellPolicyUseCaseTests()
    {
        _validator.ValidateAsync(Arg.Any<SellPolicyCommand>()).Returns(new ValidationResult());
        _useCase = new SellPolicyUseCase(_validator);
    }

    [Fact]
    public async Task SellPolicy_ShouldReturnResultValidationError_IfCommandValidationFails()
    {
        var command = new WeInsureFixture().Create<SellPolicyCommand>();
        _validator.ValidateAsync(command).Returns(new ValidationResult([new ValidationFailure("", "")]));
        
        var result = await _useCase.Execute(command);
        
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
            .With(x =>x.PolicyHolders, [_validPolicyHolder])
            .With(x => x.PolicyAddress, _validAddressDto)
            .With(x => x.Amount, 2.789m)
            .Create();
        
        var result = await _useCase.Execute(command);
        
        Assert.Null(result.Data);
        Assert.NotNull(result.Error);
        Assert.Equal(ErrorType.Domain, result.Error.Type);
        Assert.Equal("Amount cannot have more than 2 decimal places.", result.Error.Message);
    }

    [Fact]
    public async Task SellPolicy_ShouldReturnDomainError_IfPolicyAddressIsInvalid()
    {
        var invalidAddress = new AddressDto
        {
            AddressLine1 = "",
            AddressLine2 = "",
            AddressLine3 = "",
            PostCode = "",
        };
        var command = new WeInsureFixture()
            .Build<SellPolicyCommand>()
            .With(x => x.StartDate, DateOnly.FromDateTime(DateTime.UtcNow.AddDays(10)))
            .With(x =>x.PolicyHolders, [_validPolicyHolder])
            .With(x => x.Amount, 2.78m)
            .With(x => x.PolicyAddress, invalidAddress)
            .Create();
        
        var result = await _useCase.Execute(command);

        Assert.Null(result.Data);
        Assert.NotNull(result.Error);
        Assert.Equal(ErrorType.Domain, result.Error.Type);
        Assert.Equal("AddressLine1 is required", result.Error.Message);
    }
    
    [Fact]
    public async Task SellPolicy_ShouldReturnResultDomainError_IfPolicyHolderCreationCausesDomainError()
    {
        var invalidPolicyHolder = new WeInsureFixture()
            .Build<PolicyHolderDto>()
            .With(x => x.FirstName, string.Empty)
            .Create();
        var command = new WeInsureFixture()
            .Build<SellPolicyCommand>()
            .With(x => x.StartDate, DateOnly.FromDateTime(DateTime.UtcNow.AddDays(10)))
            .With(x =>x.PolicyHolders, [invalidPolicyHolder])
            .With(x => x.PolicyAddress, _validAddressDto)
            .Create();
        
        var result = await _useCase.Execute(command);
        
        Assert.Null(result.Data);
        Assert.NotNull(result.Error);
        Assert.Equal(ErrorType.Domain, result.Error.Type);
        Assert.Equal("Policy holder first name is required.", result.Error.Message);
    }


    [Fact]
    public async Task SellPolicy_ShouldReturnResultDomainError_IfPolicyCreationCausesDomainError()
    {
        var tooManyPolicyHolders = new WeInsureFixture()
            .Build<PolicyHolderDto>()
            .With(x => x.DateOfBirth, new DateOnly(1990, 1, 1))
            .CreateMany(4);
        
        var command = new WeInsureFixture()
            .Build<SellPolicyCommand>()
            .With(x => x.StartDate, DateOnly.FromDateTime(DateTime.UtcNow.AddDays(10)))
            .With(x =>x.PolicyHolders, tooManyPolicyHolders.ToList())
            .With(x => x.PolicyAddress, _validAddressDto)
            .Create();
        
        var result = await _useCase.Execute(command);
        
        Assert.Null(result.Data);
        Assert.NotNull(result.Error);
        Assert.Equal(ErrorType.Domain, result.Error.Type);
        Assert.Equal("Policy must have at least 1 policy holder and no more than 3.", result.Error.Message);
    }
}