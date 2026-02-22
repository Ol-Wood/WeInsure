using Microsoft.AspNetCore.Mvc;
using NSubstitute;
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
    private readonly PolicyController _policyController;
    private readonly SellPolicyCommand _sellPolicyCommand =  new()
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
        }
    };

    public PolicyControllerTests()
    {
        _policyController = new PolicyController(_sellPolicyUseCase);
    }
    

    [Fact]
    public async Task SellPolicy_Returns200WithCreatedPolicyInfo_WhenCreationSuccessful()
    {
        var policyId = Guid.NewGuid();
        var policyReference = Guid.NewGuid().ToString();
        
        var sellPolicyResultDto = new SoldPolicy(policyId, policyReference);
        _sellPolicyUseCase.Execute(_sellPolicyCommand).Returns(Result.Success(sellPolicyResultDto));

        var result = await _policyController.SellPolicy(_sellPolicyCommand);
        
        var expectedResponse = new SoldPolicy(policyId, policyReference);
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(expectedResponse, okResult.Value);
    }
    
    
    [Fact]
    public async Task SellPolicy_Returns400WithErrorMessage_WhenUseCaseReturnsValidationError()
    {
        const string errorMessage = "Policy holder not old enough";
        
        _sellPolicyUseCase.Execute(_sellPolicyCommand)
            .Returns(Result<SoldPolicy>.Failure(Error.Validation(errorMessage)));

        var result = await _policyController.SellPolicy(_sellPolicyCommand);
        
        var badResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal(errorMessage, badResult.Value);
    }
    
    [Fact]
    public async Task SellPolicy_Returns400WithErrorMessage_WhenUseCaseReturnsDomainError()
    {
        const string errorMessage = "Domain Error";
        
        _sellPolicyUseCase.Execute(_sellPolicyCommand)
            .Returns(Result<SoldPolicy>.Failure(Error.Domain(errorMessage)));

        var result = await _policyController.SellPolicy(_sellPolicyCommand);
        
        var badResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal(errorMessage, badResult.Value);
    }
}


