using Microsoft.Extensions.AI;
using Microsoft.SemanticKernel;

var modelId = "qwen3:4b";
var apiKey = "none";
var kernelBuilder = Kernel.CreateBuilder();

kernelBuilder.AddOpenAIChatClient(modelId, new Uri("http://localhost:11434/v1"), apiKey);

var kernel = kernelBuilder.Build();

var chatClient = kernel.GetRequiredService<IChatClient>();

var prompt = "Cuáles son las ciudades más grandes del mundo?";

Console.WriteLine("Pensando...");

bool hasFinishedThinking = false;

await foreach (var item in chatClient.GetStreamingResponseAsync(prompt))
{
    if (item.Text == "</think>")
    {
        hasFinishedThinking = true;
        continue;
    }

    if (hasFinishedThinking)
    {
        Console.Write(item);
    }
}