using Microsoft.SemanticKernel;

namespace FabuRobotics.AIAgent.Services;

public class AgentService(Kernel kernel)
{
    public async Task<string> Chat(string message)
    {
        var result = await kernel.InvokePromptAsync(message);
        return result.GetValue<string>();
    }
}