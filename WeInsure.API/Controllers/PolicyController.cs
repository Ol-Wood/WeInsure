using Microsoft.AspNetCore.Mvc;
using WeInsure.API.Models.Policy;
using WeInsure.Application.Policy.Commands;
using WeInsure.Application.Policy.UseCases.Interfaces;

namespace WeInsure.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PolicyController(ISellPolicyUseCase sellPolicyUseCase) : ControllerBase
{
    
    [HttpPost("sell")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult> SellPolicy([FromBody] SellPolicyRequest request)
    {
        var command = new SellPolicyCommand();
        
        var result = await sellPolicyUseCase.Execute(command);
        
        return Ok(result);
    }
}