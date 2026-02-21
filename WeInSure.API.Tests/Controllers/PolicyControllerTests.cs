using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using WeInsure.API.Controllers;
using WeInsure.API.Models.Policy;
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
    public async Task SellPolicy_Returns200Ok_WhenCreationSuccessful()
    {
        var request = new SellPolicyRequest();

        var result = await _policyController.SellPolicy(request);
        
        Assert.IsType<OkObjectResult>(result);
    }
}
