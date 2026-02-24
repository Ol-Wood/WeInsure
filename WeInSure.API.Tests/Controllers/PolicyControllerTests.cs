using AutoFixture;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using TestUtils.AutoFixture;
using WeInsure.API.Controllers;
using WeInsure.Application.Policy.Commands;
using WeInsure.Application.Policy.Dtos;
using WeInsure.Application.Policy.UseCases.Interfaces;
using WeInsure.Domain.Enums;
using WeInsure.Domain.Shared;

namespace WeInSure.API.Tests.Controllers;

public class PolicyControllerTests
{
    private readonly ISellPolicyUseCase _sellPolicyUseCase = Substitute.For<ISellPolicyUseCase>();
    private readonly IGetPolicyUseCase _getPolicyUseCase = Substitute.For<IGetPolicyUseCase>();
    private readonly IRenewPolicyUseCase _renewPolicyUseCase = Substitute.For<IRenewPolicyUseCase>();
    private readonly PolicyController _policyController;

    public PolicyControllerTests()
    {
        _policyController = new PolicyController(_sellPolicyUseCase, _getPolicyUseCase, _renewPolicyUseCase);
    }

    [Fact]
    public async Task SellPolicy_Returns200WithCreatedPolicyInfo_WhenCreationSuccessful()
    {
        var command = CreateSellPolicyCommand();
        var policyId = Guid.NewGuid();
        var policyReference = Guid.NewGuid().ToString();
        
        var sellPolicyResultDto = new SoldPolicy(policyId, policyReference);
        _sellPolicyUseCase.Execute(command).Returns(Result.Success(sellPolicyResultDto));

        var result = await _policyController.SellPolicy(command);
        
        var expectedResponse = new SoldPolicy(policyId, policyReference);
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(expectedResponse, okResult.Value);
    }
    
    
    [Fact]
    public async Task SellPolicy_Returns400WithErrorMessage_WhenUseCaseReturnsValidationError()
    {
        var command = CreateSellPolicyCommand();
        const string errorMessage = "Policy holder not old enough";
        
        _sellPolicyUseCase.Execute(command)
            .Returns(Result<SoldPolicy>.Failure(Error.Validation(errorMessage)));

        var result = await _policyController.SellPolicy(command);
        
        var badResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal(errorMessage, badResult.Value);
    }
    
    [Fact]
    public async Task SellPolicy_Returns400WithErrorMessage_WhenUseCaseReturnsDomainError()
    {
        var command = CreateSellPolicyCommand();
        const string errorMessage = "Domain Error";
        
        _sellPolicyUseCase.Execute(command)
            .Returns(Result<SoldPolicy>.Failure(Error.Domain(errorMessage)));

        var result = await _policyController.SellPolicy(command);
        
        var badResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal(errorMessage, badResult.Value);
    }


    [Fact]
    public async Task GetPolicy_Returns200WithPolicy_WhenUseCaseReturnsSuccess()
    {
        const string policyReference = "policyReference";
        var policy = new WeInsureFixture().Create<PolicyDto>();
        _getPolicyUseCase.Execute(policyReference).Returns(policy);
        
        var result = await _policyController.GetPolicy(policyReference);
        
        var objectResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(policy, objectResult.Value);
    }
    
    [Fact]
    public async Task GetPolicy_Return404_WhenUseCaseReturnsNull()
    {
        const string policyReference = "policyReference";
        _getPolicyUseCase.Execute(policyReference).ReturnsNull();
        
        var result = await _policyController.GetPolicy(policyReference);
        
       Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task RenewPolicy_Returns400WithErrorMessage_WhenUseCaseReturnsValidationError()
    {
        const string policyReference = "policyReference";
        var error = Error.Validation("Error renewing policy");
        _renewPolicyUseCase.
            Execute(Arg.Is<RenewPolicyCommand>(x => x.PolicyReference == policyReference))
            .Returns(Result<SoldPolicy>.Failure(error));
        
        var result = await _policyController.RenewPolicy(policyReference);
        
        var badResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal(error.Message, badResult.Value);
    }
    
    [Fact]
    public async Task RenewPolicy_Returns404WithErrorMessage_WhenUseCaseReturnsNotFoundError()
    {
        const string policyReference = "policyReference";
        var error = Error.NotFound("Policy not found");
        _renewPolicyUseCase.
            Execute(Arg.Is<RenewPolicyCommand>(x => x.PolicyReference == policyReference))
            .Returns(Result<SoldPolicy>.Failure(error));
        
        var result = await _policyController.RenewPolicy(policyReference);
        
        var badResult = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal(error.Message, badResult.Value);
    }
    
        
    [Fact]
    public async Task RenewPolicy_Returns200WithRenewedPolicyInfo_WhenUseCaseReturnsSuccessfully()
    {
        const string policyReference = "policyReference";
        var renewedPolicy = new SoldPolicy(Guid.NewGuid(), policyReference);
        _renewPolicyUseCase.
            Execute(Arg.Is<RenewPolicyCommand>(x => x.PolicyReference == policyReference))
            .Returns(Result<SoldPolicy>.Success(renewedPolicy));
        
        var result = await _policyController.RenewPolicy(policyReference);
        
        var okObject = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(renewedPolicy, okObject.Value);
    }

    private static SellPolicyCommand CreateSellPolicyCommand()
    {
        return new SellPolicyCommand
        {
            Amount = 100,
            PolicyType = PolicyType.Household,
            StartDate = DateOnly.FromDateTime(DateTime.UtcNow),
            Payment = new PaymentDto
            {
                PaymentReference = "PayRef123",
                Amount = 100,
                PaymentType = PaymentType.Card
            },
            PolicyHolders = [new PolicyHolderDto
            {
                FirstName = "John",
                LastName = "Doe",
                DateOfBirth = DateOnly.FromDateTime(new DateTime(1990, 1, 1)),
            }],
            PolicyAddress = new AddressDto
            {
                AddressLine1 = "AddressLine1",
                AddressLine2 = "AddressLine2",
                AddressLine3 = "AddressLine3",
                PostCode = "PostCode",
            },
            AutoRenew = false,
        };
    }
}


