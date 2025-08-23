using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;

var modelId = "gpt-4o";
var apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY");
var kernelBuilder = Kernel.CreateBuilder();

kernelBuilder.AddOpenAIChatClient(modelId, apiKey)
             .AddOpenAIChatCompletion(modelId, apiKey)
             .Services.AddLogging(logging =>
             {
                 logging.AddConsole();
                 logging.SetMinimumLevel(LogLevel.Trace);
             });

var kernel = kernelBuilder.Build();

var chatCompletionService = kernel.GetRequiredService<IChatCompletionService>();

var chatHistory = new ChatHistory("Eres un agente muy útil");

while (true)
{
    Console.Write("Prompt: ");
    var prompt = Console.ReadLine();

    chatHistory.AddUserMessage(prompt);

    var response = await chatCompletionService.GetChatMessageContentAsync(chatHistory);

    chatHistory.AddAssistantMessage(response.Content);

    Console.WriteLine($"\n{response.Content}\n");
}