using WeInsure.Application.Policy.Commands;
using WeInsure.Application.Policy.Dtos;
using WeInsure.Application.Policy.Validators;
using WeInsure.Domain.Enums;

namespace WeInsure.Application.Tests.Policy.Validators;

public class SellPolicyCommandValidatorTests
{
    
    private readonly SellPolicyCommand _validCommand = new()
    {
        StartDate = new DateOnly(2020, 01, 01),
        Amount = 100.00m,
        PolicyType = PolicyType.Household,
        PolicyHolders = new List<PolicyHolderDto>
        {
            new() { FirstName = "John", LastName = "Doe", DateOfBirth = new DateOnly(1990, 1, 1) }
        },
        Payment = new PaymentDto
        {
            PaymentReference = "REF123",
            Amount = 100.00m,
            PaymentType = PaymentType.Card
        },
        PolicyAddress = new AddressDto
        {
            AddressLine1 = "123 Main St",
            PostCode = "SW1A1AA"
        }
    };
    
    [Fact]
    public void Validate_ShouldReturnValidationError_WhenStartDateIsMinValue()
    {
        var validator = new SellPolicyCommandValidator();
        var command = _validCommand with { StartDate = DateOnly.MinValue };
        
        var result = validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.NotEmpty(result.Errors);
        Assert.Contains(result.Errors, e => e.PropertyName == "StartDate");
    }

    [Fact]
    public void Validate_ShouldReturnValidationError_WhenAmountIsInvalid()
    {
        var validator = new SellPolicyCommandValidator();
        var command = _validCommand with { Amount = -100 };
        
        var result = validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.NotEmpty(result.Errors);
        Assert.Contains(result.Errors, e => e.PropertyName == "Amount" && e.ErrorMessage == "Amount cannot be negative.");
    }
    
    [Fact]
    public void Validate_ShouldReturnValidationError_WhenPaymentAmountIsInvalid()
    {
        var validator = new SellPolicyCommandValidator();
        var command = _validCommand with { Payment = _validCommand.Payment with { Amount =  -100 }};

        var result = validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.NotEmpty(result.Errors);
        Assert.Contains(result.Errors, e => e.PropertyName == "Payment.Amount" && e.ErrorMessage == "Amount cannot be negative.");
    }
    
    [Fact]
    public void Validate_ShouldReturnValidationError_WhenPolicyAddressIsInvalid()
    {
        var validator = new SellPolicyCommandValidator();
        var command = _validCommand with { PolicyAddress = _validCommand.PolicyAddress with { AddressLine1 = "" } };
        var result = validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.NotEmpty(result.Errors);
        Assert.Contains(result.Errors, e => e.PropertyName == "PolicyAddress" && e.ErrorMessage == "AddressLine1 is required");
    }
    
    
    [Fact]
    public void Validate_ShouldReturnValidationError_WhenPolicyTypeIsInvalid()
    {
        var validator = new SellPolicyCommandValidator();
        var command = _validCommand with { PolicyType = (PolicyType)99 };

        var result = validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.NotEmpty(result.Errors);
        Assert.Contains(result.Errors, e => e.PropertyName == "PolicyType");
    }

    [Fact]
    public void Validate_ShouldReturnValidationError_WhenPolicyHoldersIsEmpty()
    {
        var validator = new SellPolicyCommandValidator();
        var command = _validCommand with { PolicyHolders = [] };

        var result = validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.NotEmpty(result.Errors);
        Assert.Contains(result.Errors, e => e.PropertyName == "PolicyHolders");
    }

    [Fact]
    public void Validate_ShouldReturnValid_WhenCommandIsValid()
    {
        var validator = new SellPolicyCommandValidator();

        var result = validator.Validate(_validCommand);

        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }
}
