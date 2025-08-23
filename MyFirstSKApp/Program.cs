using Microsoft.Extensions.AI;
using Microsoft.SemanticKernel;

var modelId = "gpt-4o";
var apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY");
var kernelBuilder = Kernel.CreateBuilder();

kernelBuilder.AddOpenAIChatClient(modelId, apiKey);

var kernel = kernelBuilder.Build();

var chatClient = kernel.GetRequiredService<IChatClient>();

var messages = new List<ChatMessage>
{
    new ChatMessage(ChatRole.System, "Eres un agente muy útil.")
};

while (true)
{
    Console.WriteLine("Prompt: ");
    var prompt = Console.ReadLine();

    messages.Add(new ChatMessage(ChatRole.User, prompt));

    var response = await chatClient.GetResponseAsync(messages);

    messages.Add(new ChatMessage(ChatRole.Assistant, response.Text));

    Console.WriteLine($"\n{response.Text}\n");
}