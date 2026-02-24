using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using WeInsure.API.Controllers;
using WeInsure.Application.Policy.Dtos;
using WeInsure.Application.Policy.UseCases;
using WeInsure.Application.Policy.UseCases.Interfaces;
using WeInsure.Application.Services;
using WeInsure.Domain.Entities;
using WeInsure.Domain.Enums;
using WeInsure.Domain.Repositories;
using WeInsure.Domain.Shared;
using WeInsure.Domain.ValueObjects;

namespace WeInsure.Feature.Tests.Policy.RenewPolicy;

public class RenewPolicyFeatureTests
{
    private readonly PolicyController _controller;
    private readonly IPolicyRepository _policyRepository = Substitute.For<IPolicyRepository>();
    
    public RenewPolicyFeatureTests()
    {
        _controller = new PolicyController(
            Substitute.For<ISellPolicyUseCase>(),
            Substitute.For<IGetPolicyUseCase>(),
            new RenewPolicyUseCase(_policyRepository, new PolicyReferenceGenerator(_policyRepository), new IdGenerator()));
    }

    [Fact]
    public async Task RenewPolicy_Returns400_WhenRenewIsAttemptedMoreThan30DaysBeforeCurrentPolicyExpiration()
    {
        var policy = CreatePolicy(DateOnly.FromDateTime(DateTime.UtcNow.AddDays(31)));
        _policyRepository.GetByReference(Arg.Any<string>()).Returns(policy);

        var result = await _controller.RenewPolicy(policy.Reference.Value);

        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Contains("Too early", badRequestResult.Value?.ToString());
    }

    [Fact]
    public async Task RenewPolicy_Returns400_WhenPolicyHasExpired()
    {
        var policy = CreatePolicy(DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-2)));
        _policyRepository.GetByReference(Arg.Any<string>()).Returns(policy);

        var result = await _controller.RenewPolicy(policy.Reference.Value);

        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Contains("expired", badRequestResult.Value?.ToString());
    }

    [Fact]
    public async Task RenewPolicy_Returns404_WhenPolicyNotFound()
    {
        var reference = PolicyReference.Create().Value;
        _policyRepository.GetByReference(Arg.Any<string>()).ReturnsNull();

        var result = await _controller.RenewPolicy(reference);

        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Contains("does not exist", notFoundResult.Value?.ToString());
    }

    [Fact]
    public async Task RenewPolicy_Returns200_WithNewPolicyDetails_WhenSuccessful()
    {
        var policy = CreatePolicy(DateOnly.FromDateTime(DateTime.UtcNow.AddDays(20)));
        _policyRepository.GetByReference(Arg.Any<string>()).Returns(policy);

        var result = await _controller.RenewPolicy(policy.Reference.Value);

        var okResult = Assert.IsType<OkObjectResult>(result);
        var soldPolicy = Assert.IsType<SoldPolicy>(okResult.Value);
        Assert.NotEqual(Guid.Empty, soldPolicy.PolicyId);
        Assert.NotNull(soldPolicy.PolicyReference);
    }
    

    private static Domain.Entities.Policy CreatePolicy(DateOnly endDate)
    {
        var policyId = Guid.NewGuid();
        var policyReference = PolicyReference.Create();
        var moneyAmount = Money.Create(2000).OrThrow();
        var policyHolder = PolicyHolder.Create(
            Guid.NewGuid(),
            policyId,
            "John",
            "Doe",
            new DateOnly(1990, 01, 01)).OrThrow();
        var address = Address.Create("Line1", "Line2", "Line3", "M34 7ER").OrThrow();
        var insuredProperty = InsuredProperty.Create(Guid.NewGuid(), policyId, address);
        var payment = Payment.Create(Guid.NewGuid(), policyId, PaymentType.Card, moneyAmount, "REF").OrThrow();

        var startDate = endDate.AddYears(-1);

        var policy = Domain.Entities.Policy.Create(
            policyId,
            policyReference,
            startDate,
            PolicyType.Household,
            [policyHolder],
            moneyAmount,
            insuredProperty,
            payment,
            true,
            startDate);

        return policy.OrThrow();
    }
}