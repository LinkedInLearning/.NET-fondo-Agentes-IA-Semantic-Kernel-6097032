using FabuRobotics.AIAgent.Services;
using Microsoft.AspNetCore.Mvc;

namespace FabuRobotics.AIAgent.Controllers;

[ApiController]
[Route("[controller]")]
public class AgentController(AgentService agentService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] string message)
    {
        var result = await agentService.Chat(message);
        return Ok(result);
    }
}