using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using WeInsure.API.Controllers;
using WeInsure.Application.Policy.UseCases;
using WeInsure.Application.Policy.UseCases.Interfaces;
using WeInsure.Domain.ValueObjects;

namespace WeInsure.Feature.Tests.Policy.GetPolicy;

public class GetPolicyFeatureTests
{
    private readonly PolicyController _controller;

    public GetPolicyFeatureTests()
    {
        _controller = new PolicyController(Substitute.For<ISellPolicyUseCase>(), new GetPolicyUseCase());
    }
    
    [Fact]
    public async Task GetPolicyFeature_Returns200_WithPolicy_WhenSuccessful()
    {
        var policyReference = PolicyReference.Create();
        
        var result = await _controller.GetPolicy(policyReference.Value);
        
        Assert.IsType<OkObjectResult>(result);
    }
}