using Microsoft.AspNetCore.Mvc;
using WeInsure.Application.Policy.Commands;
using WeInsure.Application.Policy.UseCases.Interfaces;
using WeInsure.Domain.Shared;

namespace WeInsure.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PolicyController(ISellPolicyUseCase sellPolicyUseCase) : ControllerBase
{
    
    [HttpPost("sell")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> SellPolicy([FromBody] SellPolicyCommand command)
    {
        var result = await sellPolicyUseCase.Execute(command);

        if (result.IsSuccess)
        {
            return Ok(result.Data);
        }

        return result.Error?.Type switch
        {
            ErrorType.Validation => BadRequest(result.Error.Message),
            ErrorType.Domain => BadRequest(result.Error.Message),
            _ => StatusCode(500)
        };
    }
}