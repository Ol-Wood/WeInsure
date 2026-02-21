using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using WeInsure.API.Controllers;
using WeInsure.Application.Policy.Commands;
using WeInsure.Application.Policy.Dtos;
using WeInsure.Application.Policy.UseCases.Interfaces;
using WeInsure.Domain.Enums;

namespace WeInSure.API.Tests.Controllers;

public class PolicyControllerTests
{
    private readonly ISellPolicyUseCase _sellPolicyUseCase = Substitute.For<ISellPolicyUseCase>();
    private readonly PolicyController _policyController;

    public PolicyControllerTests()
    {
        _policyController = new PolicyController(_sellPolicyUseCase);
    }
    

    [Fact]
    public async Task SellPolicy_Returns200WithCreatedPolicyInfo_WhenCreationSuccessful()
    {
        var policyId = Guid.NewGuid();
        var policyReference = Guid.NewGuid().ToString();
        var command = new SellPolicyCommand
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
            }]
        };
        
        var sellPolicyResultDto = new SellPolicyResultDto(policyId, policyReference);
        _sellPolicyUseCase.Execute(command).Returns(sellPolicyResultDto);

        var result = await _policyController.SellPolicy(command);
        
        var expectedResponse = new SellPolicyResultDto(policyId, policyReference);
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(expectedResponse, okResult.Value);
    }
}


