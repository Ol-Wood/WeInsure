using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using WeInsure.API.Controllers;
using WeInsure.Application.Policy.Commands;
using WeInsure.Application.Policy.Dtos;
using WeInsure.Application.Policy.UseCases;
using WeInsure.Application.Policy.Validators;
using WeInsure.Application.Services;
using WeInsure.Domain.Enums;
using WeInsure.Domain.Repositories;

namespace WeInsure.Feature.Tests.Policy.SellPolicy;

public class SellPolicyFeatureTests
{
    private readonly PolicyController _controller;
    private readonly IPolicyRepository _policyRepository = Substitute.For<IPolicyRepository>();

    public SellPolicyFeatureTests()
    {
        var validator = new SellPolicyCommandValidator();
        var sellPolicyUseCase = new SellPolicyUseCase(validator, new IdGenerator(), _policyRepository,
            new PolicyReferenceGenerator());
        _controller = new PolicyController(sellPolicyUseCase);
    }

    [Fact]
    public async Task SellPolicyShould_SuccessfullyCreatePolicy()
    {
        var command = new SellPolicyCommand
        {
            Amount = 100,
            PolicyType = PolicyType.Household,
            StartDate = DateOnly.FromDateTime(DateTime.UtcNow),
            Payment = new PaymentDto
            {
                PaymentReference = "PayRef123",
                Amount = 100,
                PaymentType = PaymentType.Card
            },
            PolicyHolders =
            [
                new PolicyHolderDto
                {
                    FirstName = "John",
                    LastName = "Doe",
                    DateOfBirth = DateOnly.FromDateTime(new DateTime(1990, 1, 1)),
                }
            ],
            PolicyAddress = new AddressDto
            {
                AddressLine1 = "AddressLine1",
                AddressLine2 = "AddressLine2",
                AddressLine3 = "AddressLine3",
                PostCode = "PostCode",
            },
            AutoRenew = false,
        };

        var result = await _controller.SellPolicy(command);

        Assert.IsType<OkObjectResult>(result);
    }
}