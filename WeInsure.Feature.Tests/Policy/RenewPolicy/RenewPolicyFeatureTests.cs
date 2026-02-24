using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using WeInsure.API.Controllers;
using WeInsure.Application.Policy.UseCases;
using WeInsure.Application.Policy.UseCases.Interfaces;
using WeInsure.Domain.ValueObjects;

namespace WeInsure.Feature.Tests.Policy.RenewPolicy;

public class RenewPolicyFeatureTests
{
    private readonly PolicyController _controller;
    // Reqs
    // Can only be renewed 30 days before policy end
    // Policy cannot be renewed after end date
    // 

    public RenewPolicyFeatureTests()
    {
        _controller = new PolicyController(Substitute.For<ISellPolicyUseCase>(), Substitute.For<IGetPolicyUseCase>(),
            new RenewPolicyUseCase());
    }

    [Fact]
    public async Task RenewPolicy_Returns400_WhenRenewIsAttemptedMoreThan30DaysBeforeCurrentPolicyExpiration()
    {
        var reference = PolicyReference.Create().Value;

        var result = await _controller.RenewPolicy(reference);
        Assert.IsType<BadRequestObjectResult>(result);
    }
}