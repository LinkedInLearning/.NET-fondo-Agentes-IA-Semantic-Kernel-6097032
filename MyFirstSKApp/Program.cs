using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Microsoft.SemanticKernel.Plugins.Core;
using System.ComponentModel;
using System.Diagnostics;

var modelId = "gpt-4o";
var apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY");
var kernelBuilder = Kernel.CreateBuilder();

kernelBuilder.AddOpenAIChatCompletion(modelId, apiKey)
             .Plugins.AddFromType<TimePlugin>()
                     .AddFromType<ProcessPlugin>()
             .Services.AddLogging(logging =>
             {
                 logging.AddConsole();
                 logging.SetMinimumLevel(LogLevel.Trace);
             });

var kernel = kernelBuilder.Build();

var promptTemplate = """
        Eres un agente muy útil.

        #Histórico de mensajes
        {{$history}}

        Assistant:
    """;

var settings = new OpenAIPromptExecutionSettings() 
{ 
    Temperature = 0.7f, 
    MaxTokens = 1000,
    FunctionChoiceBehavior = FunctionChoiceBehavior.Auto()
};

var history = new List<Message>();

while (true)
{
    Console.Write("Prompt: ");
    var message = Console.ReadLine();

    var kernelArgs = new KernelArguments(settings)
    {
        { "history", history }
    };

    history.Add(new Message("User", message));

    var result = await kernel.InvokePromptAsync(promptTemplate,
                                                kernelArgs);

    var resultContent = result.GetValue<string>();

    history.Add(new Message("Assistant", resultContent));
    
    Console.WriteLine($"\n{resultContent}\n");

}

public record Message(string Role, string Content);


public class ProcessPlugin
{
    [KernelFunction]
    [Description("Regresa la información de todos los procesos de la máquina.")]
    public IEnumerable<string> GetProcesses()
    {
        return Process.GetProcesses().Select(p => $"Id:{p.Id} ProcessName:{p.ProcessName}");
    }
}