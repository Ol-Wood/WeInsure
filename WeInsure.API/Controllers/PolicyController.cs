using Microsoft.AspNetCore.Mvc;
using WeInsure.Application.Policy.Commands;
using WeInsure.Application.Policy.UseCases.Interfaces;
using WeInsure.Domain.Shared;

namespace WeInsure.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PolicyController(
    ISellPolicyUseCase sellPolicyUseCase,
    IGetPolicyUseCase getPolicyUseCase,
    IRenewPolicyUseCase renewPolicyUseCase) : ControllerBase
{
    [HttpPost("sell")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> SellPolicy([FromBody] SellPolicyCommand command)
    {
        var result = await sellPolicyUseCase.Execute(command);

        if (!result.IsSuccess)
        {
            return result.Error?.Type switch
            {
                ErrorType.Validation => BadRequest(result.Error.Message),
                ErrorType.Domain => BadRequest(result.Error.Message),
                _ => StatusCode(500)
            };
        }
        
        return Ok(result.Data);
       
    }

    [HttpGet("{policyReference}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> GetPolicy(string policyReference)
    {
        var result = await getPolicyUseCase.Execute(policyReference);

        if (result is null)
        {
            return NotFound();
        }

        return Ok(result);
    }

    [HttpPost("{policyReference}")]
    public async Task<ActionResult> RenewPolicy(string reference)
    {
        var command = new RenewPolicyCommand(reference);
        var result = await renewPolicyUseCase.Execute(command);

        if (!result.IsSuccess)
        {
            return result.Error.Type switch
            {
                ErrorType.Validation => BadRequest(result.Error.Message),
                ErrorType.Domain => BadRequest(result.Error.Message),
                ErrorType.NotFound => NotFound(result.Error.Message),
                _ => StatusCode(500)
            };
        }
        
        
        return Ok(result.Data);
    }
}