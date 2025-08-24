using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.OpenAI;

var modelId = "gpt-4o";
var apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY");
var kernelBuilder = Kernel.CreateBuilder();

kernelBuilder.AddOpenAIChatCompletion(modelId, apiKey)
             .Services.AddLogging(logging =>
             {
                 logging.AddConsole();
                 logging.SetMinimumLevel(LogLevel.Trace);
             });

var kernel = kernelBuilder.Build();

var settings = new OpenAIPromptExecutionSettings() { Temperature = 0.7f, MaxTokens = 1000 };

var kernelArgs = new KernelArguments(settings);

while (true)
{
    Console.Write("Prompt: ");
    var prompt = Console.ReadLine();

    var result = await kernel.InvokePromptAsync(prompt, kernelArgs);

    Console.WriteLine($"\n{result.GetValue<string>()}\n");
}