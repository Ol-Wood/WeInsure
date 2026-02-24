using NSubstitute;
using NSubstitute.ReturnsExtensions;
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

    public RenewPolicyUseCastTests()
    {
        _useCase = new RenewPolicyUseCase(_policyRepository);
    }
    
    [Fact]
    public async Task RenewPolicy_ReturnsNotFoundError_WhenPolicyDoesNotExist()
    {
        var policyRef = PolicyReference.Create();
        _policyRepository.GetByReference(policyRef.Value).ReturnsNull();

        var result = await _useCase.Execute(new RenewPolicyCommand(policyRef.Value));
        
        Assert.False(result.IsSuccess);
        Assert.Equivalent(Error.NotFound($"Policy {policyRef.Value} does not exist."), result.Error);
    }
}