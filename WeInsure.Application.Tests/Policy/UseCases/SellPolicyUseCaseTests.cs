using AutoFixture;
using FluentValidation;
using FluentValidation.Results;
using NSubstitute;
using TestUtils.AutoFixture;
using WeInsure.Application.Policy.Commands;
using WeInsure.Application.Policy.Dtos;
using WeInsure.Application.Policy.UseCases;
using WeInsure.Application.Services;
using WeInsure.Domain.Enums;
using WeInsure.Domain.Repositories;
using WeInsure.Domain.Shared;
using WeInsure.Domain.ValueObjects;

namespace WeInsure.Application.Tests.Policy.UseCases;

public class SellPolicyUseCaseTests
{
    private readonly Guid _policyId = Guid.CreateVersion7();
    private readonly IValidator<SellPolicyCommand> _validator = Substitute.For<IValidator<SellPolicyCommand>>();
    private readonly IIdGenerator _idGenerator = Substitute.For<IIdGenerator>();
    private readonly SellPolicyUseCase _useCase;
    private readonly IPolicyRepository _policyRepository = Substitute.For<IPolicyRepository>();

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
        _idGenerator.Generate().Returns(_policyId);
        _useCase = new SellPolicyUseCase(_validator, _idGenerator, _policyRepository);
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
            .With(x => x.PolicyHolders, [_validPolicyHolder])
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
            .With(x => x.PolicyHolders, [_validPolicyHolder])
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
    public async Task SellPolicy_ShouldReturnDomainError_IfPaymentIsInvalid()
    {
        var invalidPayment = new PaymentDto
        {
            Amount = 2.78m,
            PaymentReference = "",
            PaymentType = PaymentType.Card
        };
        var command = new WeInsureFixture()
            .Build<SellPolicyCommand>()
            .With(x => x.StartDate, DateOnly.FromDateTime(DateTime.UtcNow.AddDays(10)))
            .With(x => x.PolicyHolders, [_validPolicyHolder])
            .With(x => x.Amount, 2.78m)
            .With(x => x.Payment, invalidPayment)
            .With(x => x.PolicyAddress, _validAddressDto)
            .Create();

        var result = await _useCase.Execute(command);

        Assert.Null(result.Data);
        Assert.NotNull(result.Error);
        Assert.Equal(ErrorType.Domain, result.Error.Type);
        Assert.Equal("Payment reference is required", result.Error.Message);
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
            .With(x => x.PolicyHolders, [invalidPolicyHolder])
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
            .With(x => x.PolicyHolders, tooManyPolicyHolders.ToList())
            .With(x => x.PolicyAddress, _validAddressDto)
            .Create();

        var result = await _useCase.Execute(command);

        Assert.Null(result.Data);
        Assert.NotNull(result.Error);
        Assert.Equal(ErrorType.Domain, result.Error.Type);
        Assert.Equal("Policy must have at least 1 policy holder and no more than 3.", result.Error.Message);
    }

    [Fact]
    public async Task SellPolicy_ShouldCreateAndSavePolicy_IfPolicyCreationIsValid()
    {
        var command = new WeInsureFixture()
            .Build<SellPolicyCommand>()
            .With(x => x.StartDate, DateOnly.FromDateTime(DateTime.UtcNow.AddDays(10)))
            .With(x => x.PolicyHolders, [_validPolicyHolder])
            .With(x => x.PolicyAddress, _validAddressDto)
            .With(x => x.PolicyType, PolicyType.Household)
            .Create();

        var result = await _useCase.Execute(command);

        await _policyRepository.Received().Add(Arg.Is<Domain.Entities.Policy>(x =>
            x.Id == _policyId &&
            x.StartDate == command.StartDate &&
            x.Price == Money.Create(command.Amount).Data &&
            x.PolicyHolders.Count == command.PolicyHolders.Count &&
            x.InsuredProperty.Address.AddressLine1 == command.PolicyAddress.AddressLine1
        ));
        
        Assert.True(result.IsSuccess);
        Assert.Null(result.Error);
        Assert.Equal(_policyId, result.Data.PolicyId);
        Assert.Equal("Ref", result.Data.PolicyReference);
    }
}