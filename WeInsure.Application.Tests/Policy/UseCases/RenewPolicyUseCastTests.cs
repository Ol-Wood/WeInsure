using NSubstitute;
using NSubstitute.ReturnsExtensions;
using TestUtils.TestData;
using WeInsure.Application.Policy.Commands;
using WeInsure.Application.Policy.UseCases;
using WeInsure.Domain.Repositories;
using WeInsure.Domain.Shared;
using WeInsure.Domain.ValueObjects;

namespace WeInsure.Application.Tests.Policy.UseCases;

public class RenewPolicyUseCastTests
{
    private readonly RenewPolicyUseCase _useCase;
    private readonly IPolicyRepository _policyRepository = Substitute.For<IPolicyRepository>();
    private readonly string _policyReference = PolicyReference.Create().Value;

    public RenewPolicyUseCastTests()
    {
        _useCase = new RenewPolicyUseCase(_policyRepository);
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
        var policy = PolicyDataGenerator.CreatePolicy();
        _policyRepository.GetByReference(_policyReference).Returns(policy);
        
        var result = await _useCase.Execute(new RenewPolicyCommand(_policyReference));
        
        Assert.False(result.IsSuccess);
        Assert.Equivalent(Error.Domain("Too early for policy renewal"), result.Error);
    }
}