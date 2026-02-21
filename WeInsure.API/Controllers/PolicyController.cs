using Microsoft.AspNetCore.Mvc;
using WeInsure.API.Models.Policy;
using WeInsure.Application.Policy.UseCases.Interfaces;

namespace WeInsure.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PolicyController(ISellPolicyUseCase sellPolicyUseCase) : ControllerBase
{
    public async Task<ActionResult> SellPolicy(SellPolicyRequest request)
    {
        throw new NotImplementedException();
    }
}