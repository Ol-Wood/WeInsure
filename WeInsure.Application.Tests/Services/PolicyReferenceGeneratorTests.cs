using NSubstitute;
using WeInsure.Application.Exceptions;
using WeInsure.Application.Services;
using WeInsure.Domain.Repositories;
using WeInsure.Domain.ValueObjects;

namespace WeInsure.Application.Tests.Services;

public class PolicyReferenceGeneratorTests
{
    private readonly IPolicyRepository _policyRepository = Substitute.For<IPolicyRepository>();

    [Fact]
    public async Task Generate_ShouldGeneratePolicyReference_AndIfUniqueThenReturn()
    {
        _policyRepository.Exists(Arg.Any<PolicyReference>()).Returns(false);
        var generator = new PolicyReferenceGenerator(_policyRepository);
        
        var result = await generator.Generate();
        
        await _policyRepository.Received(1).Exists(Arg.Any<PolicyReference>());
        Assert.IsType<PolicyReference>(result);
    }
    
    [Fact]
    public async Task Generate_ShouldGeneratePolicyReference_AndIfNotUniqueThenRetry()
    {
        _policyRepository.Exists(Arg.Any<PolicyReference>()).Returns(true, false);
        var generator = new PolicyReferenceGenerator(_policyRepository);
        
        var result = await generator.Generate();
        
        await _policyRepository.Received(2).Exists(Arg.Any<PolicyReference>());
        Assert.IsType<PolicyReference>(result);
    }

    [Fact]
    public async Task Generate_ShouldGeneratePolicyReference_IfNotUniqueAfter3Attempts_ThrowsException()
    {
        _policyRepository.Exists(Arg.Any<PolicyReference>()).Returns(true, true, true, false);
        var generator = new PolicyReferenceGenerator(_policyRepository);
        
        var act = () => generator.Generate();
        
        await Assert.ThrowsAsync<PolicyReferenceGenerationException>(act);
        await _policyRepository.Received(3).Exists(Arg.Any<PolicyReference>());
    }
}