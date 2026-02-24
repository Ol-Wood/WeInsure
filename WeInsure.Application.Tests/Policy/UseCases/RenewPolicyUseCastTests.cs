using NSubstitute;
using NSubstitute.ReturnsExtensions;
using WeInsure.Application.Policy.Commands;
using WeInsure.Application.Policy.UseCases;
using WeInsure.Application.Services;
using WeInsure.Domain.Entities;
using WeInsure.Domain.Enums;
using WeInsure.Domain.Repositories;
using WeInsure.Domain.Shared;
using WeInsure.Domain.ValueObjects;

namespace WeInsure.Application.Tests.Policy.UseCases;

public class RenewPolicyUseCastTests
{
    private readonly RenewPolicyUseCase _useCase;
    private readonly IPolicyRepository _policyRepository = Substitute.For<IPolicyRepository>();
    private readonly IPolicyReferenceGenerator _policyReferenceGenerator = Substitute.For<IPolicyReferenceGenerator>();
    private readonly IIdGenerator _idGenerator = Substitute.For<IIdGenerator>();
    private readonly string _policyReference = PolicyReference.Create().Value;

    public RenewPolicyUseCastTests()
    {
        _policyReferenceGenerator.Generate().Returns(PolicyReference.Create());
        _idGenerator.Generate().Returns(Guid.CreateVersion7());
        _useCase = new RenewPolicyUseCase(_policyRepository, _policyReferenceGenerator, _idGenerator);
    }

    [Fact]
    public async Task RenewPolicy_ReturnsNotFoundError_WhenPolicyDoesNotExist()
    {
        _policyRepository.GetByReference(_policyReference).ReturnsNull();

        var result = await _useCase.Execute(new RenewPolicyCommand(_policyReference));

        Assert.False(result.IsSuccess);
        Assert.Equivalent(Error.NotFound("Policy does not exist."), result.Error);
    }


    [Fact]
    public async Task RenewPolicy_ReturnsDomainError_WhenPolicyCannotBeRenewed()
    {
        var policy = CreatePolicy();
        _policyRepository.GetByReference(_policyReference).Returns(policy);

        var result = await _useCase.Execute(new RenewPolicyCommand(_policyReference));

        Assert.False(result.IsSuccess);
        Assert.Equivalent(Error.Domain("Too early for policy renewal"), result.Error);
    }

    [Fact]
    public async Task RenewPolicy_ReturnsRenewedPolicy_WhenPolicyRenewedSuccessfully()
    {
        var dateWhenPolicyCreated = DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-1));
        var policy = CreatePolicy(dateWhenPolicyCreated, dateWhenPolicyCreated);
        var renewedPolicyReference = PolicyReference.Create();
        var renewedPolicyId = Guid.CreateVersion7();
        _policyReferenceGenerator.Generate().Returns(renewedPolicyReference);
        _idGenerator.Generate().Returns(renewedPolicyId);
        _policyRepository.GetByReference(_policyReference).Returns(policy);
        var result = await _useCase.Execute(new RenewPolicyCommand(_policyReference));

       await  _policyRepository.Received(1).Add(Arg.Is<Domain.Entities.Policy>(x => 
           x.Id == renewedPolicyId && 
           x.Reference == renewedPolicyReference));
        Assert.True(result.IsSuccess);
        Assert.Equal(renewedPolicyReference.Value, result.Data.PolicyReference);
        Assert.Equal(renewedPolicyId, result.Data.PolicyId);
    }

    private static Domain.Entities.Policy CreatePolicy(DateOnly? startDate = null, DateOnly? currentDate = null)
    {
        var policyId = Guid.NewGuid();
        var policyReference = PolicyReference.Create();
        var moneyAmount = Money.Create(2000).OrThrow();
        var policyHolder =
            PolicyHolder.Create(Guid.NewGuid(), policyId, "John", "Doe", new DateOnly(1990, 01, 01)).OrThrow();
        var address = Address.Create("Line1", "Line2", "Line3", "M34 7ER").OrThrow();
        var insuredProperty = InsuredProperty.Create(Guid.NewGuid(), policyId, address);
        var payment = Payment.Create(Guid.NewGuid(), policyId, PaymentType.Card, moneyAmount, "REF").OrThrow();

        var policy = Domain.Entities.Policy.Create(
            policyId,
            policyReference,
            startDate ?? DateOnly.FromDateTime(DateTime.UtcNow),
            PolicyType.Household,
            [policyHolder],
            moneyAmount,
            insuredProperty,
            payment,
            true,
            currentDate ?? DateOnly.FromDateTime(DateTime.UtcNow));

        return policy.OrThrow();
    }
}