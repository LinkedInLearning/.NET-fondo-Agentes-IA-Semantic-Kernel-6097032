using Microsoft.AspNetCore.Mvc;
using Microsoft.SemanticKernel;

namespace FabuRobotics.AIAgent.Controllers;

[ApiController]
[Route("[controller]")]
public class AgentController(Kernel kernel) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] string message)
    {
        var result = await kernel.InvokePromptAsync(message);
        return Ok(result.GetValue<string>());
    }
}