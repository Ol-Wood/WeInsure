using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using WeInsure.API.Controllers;
using WeInsure.Application.Policy.Mappers;
using WeInsure.Application.Policy.UseCases;
using WeInsure.Application.Policy.UseCases.Interfaces;
using WeInsure.Domain.Entities;
using WeInsure.Domain.Enums;
using WeInsure.Domain.Repositories;
using WeInsure.Domain.Shared;
using WeInsure.Domain.ValueObjects;

namespace WeInsure.Feature.Tests.Policy.GetPolicy;

public class GetPolicyFeatureTests
{
    private readonly PolicyController _controller;
    private readonly IPolicyRepository _policyRepository = Substitute.For<IPolicyRepository>();

    public GetPolicyFeatureTests()
    {
        _controller =
            new PolicyController(Substitute.For<ISellPolicyUseCase>(), new GetPolicyUseCase(_policyRepository),
                Substitute.For<IRenewPolicyUseCase>());
    }

    [Fact]
    public async Task GetPolicyFeature_Returns200_WithPolicy_WhenSuccessful()
    {
        var policy = CreatePolicy();
        var policyReference = PolicyReference.Create().Value;
        _policyRepository.GetByReference(policyReference).Returns(policy);

        var result = await _controller.GetPolicy(policyReference);

        var objectResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equivalent(policy.ToDto(), objectResult.Value);
    }

    [Fact]
    public async Task GetPolicyFeature_Returns404NotFound_WhenPolicyNotFound()
    {
        var policyReference = PolicyReference.Create().Value;
        _policyRepository.GetByReference(policyReference).ReturnsNull();

        var result = await _controller.GetPolicy(policyReference);

        Assert.IsType<NotFoundResult>(result);
    }


    private static Domain.Entities.Policy CreatePolicy()
    {
        var policyId = Guid.NewGuid();
        var policyReference = PolicyReference.Create();
        var moneyAmount = Money.Create(2000).OrThrow();
        var policyHolder =
            PolicyHolder.Create(Guid.NewGuid(), policyId, "John", "Doe", new DateOnly(1990, 01, 01)).OrThrow();
        var address = Address.Create("Line1", "Line2", "Line3", "M34 7ER").OrThrow();
        var insuredProperty = InsuredProperty.Create(Guid.NewGuid(), policyId, address);
        var payment = Payment.Create(Guid.NewGuid(), policyId, PaymentType.Card, moneyAmount, "REF").OrThrow();

        var policy = Domain.Entities.Policy.Create(policyId, policyReference, DateOnly.FromDateTime(DateTime.UtcNow),
            PolicyType.Household, [policyHolder], moneyAmount, insuredProperty, payment, true, DateOnly.FromDateTime(DateTime.UtcNow));

        return policy.OrThrow();
    }
}