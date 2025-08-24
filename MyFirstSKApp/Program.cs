using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;

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

while (true)
{
    Console.Write("Prompt: ");
    var prompt = Console.ReadLine();

    var result = await kernel.InvokePromptAsync(prompt);

    Console.WriteLine($"\n{result.GetValue<string>()}\n");
}