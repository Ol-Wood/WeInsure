using NSubstitute;
using NSubstitute.ReturnsExtensions;
using WeInsure.Application.Policy.UseCases;
using WeInsure.Domain.Entities;
using WeInsure.Domain.Enums;
using WeInsure.Domain.Repositories;
using WeInsure.Domain.Shared;
using WeInsure.Domain.ValueObjects;

namespace WeInsure.Application.Tests.Policy.UseCases;

public class GetPolicyUseCaseTests
{
    private readonly IPolicyRepository _policyRepository = Substitute.For<IPolicyRepository>();
    private readonly GetPolicyUseCase _useCase;

    public GetPolicyUseCaseTests()
    {
        _useCase = new GetPolicyUseCase(_policyRepository);
    }

    [Fact]
    public async Task ReturnsNull_WhenPolicyIsNotFound()
    {
        const string reference = "reference";
        _policyRepository.GetByReference(reference).ReturnsNull();

        var result = await _useCase.Execute(reference);

        Assert.Null(result);
    }

    [Fact]
    public async Task ReturnsPolicy_WhenPolicyIsFound()
    {
        const string reference = "reference";
        var policy = CreatePolicy();
        _policyRepository.GetByReference(reference).Returns(policy);

        var result = await _useCase.Execute(reference);

        Assert.NotNull(result);
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
            PolicyType.Household, [policyHolder], moneyAmount, insuredProperty, payment);

        return policy.OrThrow();
    }
}