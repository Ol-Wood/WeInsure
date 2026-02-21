using Microsoft.AspNetCore.Mvc;
using WeInsure.API.Controllers;
using WeInsure.API.Models.Policy;

namespace WeInsure.Feature.Tests.Policy.SellPolicy;

public class SellPolicyFeatureTests
{
    [Fact]
    public async Task SellPolicyShould_SuccessfullyCreatePolicy()
    {
        var request = new SellPolicyRequest();
        var controller = new PolicyController();

        var result = await controller.SellPolicy(request);
        
        Assert.IsType<OkObjectResult>(result);
    }
}

