using Microsoft.AspNetCore.Mvc;
using WeInsure.API.Controllers;
using WeInsure.API.Models.Policy;
using WeInsure.Application.Policy.UseCases;

namespace WeInsure.Feature.Tests.Policy.SellPolicy;

public class SellPolicyFeatureTests
{
    [Fact]
    public async Task SellPolicyShould_SuccessfullyCreatePolicy()
    {
        var request = new SellPolicyRequest();
        var sellPolicyUseCase = new SellPolicyUseCase();
        var controller = new PolicyController(sellPolicyUseCase);

        var result = await controller.SellPolicy(request);
        
        Assert.IsType<OkObjectResult>(result);
    }
}

