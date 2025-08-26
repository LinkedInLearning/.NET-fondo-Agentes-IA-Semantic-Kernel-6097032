using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.OpenAI;

namespace FabuRobotics.AIAgent.Services;

public class AgentService(Kernel kernel)
{
    private static OpenAIPromptExecutionSettings settings = new()
    {
        FunctionChoiceBehavior = FunctionChoiceBehavior.Auto()
    };

    private static KernelArguments arguments = new(settings);

    public async Task<string> Chat(string message)
    {
        var result = await kernel.InvokePromptAsync(message, arguments);
        return result.GetValue<string>();
    }
}