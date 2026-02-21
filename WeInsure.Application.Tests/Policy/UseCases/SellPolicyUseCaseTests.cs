using AutoFixture;
using WeInsure.Application.Policy.Commands;
using WeInsure.Application.Policy.UseCases;
using WeInsure.Application.Tests.AutoFixture;
using WeInsure.Domain.Shared;

namespace WeInsure.Application.Tests.Policy.UseCases;

public class SellPolicyUseCaseTests
{

    [Fact]
    public async Task SellPolicy_ShouldReturnResultValidationError_IfCommandValidationFails()
    {
        var command = new WeInsureFixture().Create<SellPolicyCommand>();
        var useCase = new SellPolicyUseCase();
        
        var result = await useCase.Execute(command);
        
        Assert.Null(result.Data);
        Assert.NotNull(result.Error);
        Assert.Equal(ErrorType.Validation, result.Error.Type);
    }
}