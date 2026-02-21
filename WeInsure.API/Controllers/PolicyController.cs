using Microsoft.AspNetCore.Mvc;
using WeInsure.API.Models.Policy;

namespace WeInsure.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PolicyController : ControllerBase
{
    public async Task<ActionResult> SellPolicy(SellPolicyRequest request)
    {
        throw new NotImplementedException();
    }
}