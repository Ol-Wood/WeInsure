using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using WeInsure.API.Controllers;
using WeInsure.API.Models.Policy;
using WeInsure.Application.Policy.Commands;
using WeInsure.Application.Policy.Dtos;
using WeInsure.Application.Policy.UseCases.Interfaces;

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
        var sellPolicyResultDto = new SellPolicyResultDto(policyId, policyReference);
        _sellPolicyUseCase.Execute(Arg.Any<SellPolicyCommand>()).Returns(sellPolicyResultDto);

        var result = await _policyController.SellPolicy(new SellPolicyRequest());
        
        var expectedResponse = new SellPolicyResultDto(policyId, policyReference);
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(expectedResponse, okResult.Value);
    }
}


