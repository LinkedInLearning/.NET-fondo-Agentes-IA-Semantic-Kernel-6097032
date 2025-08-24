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

var promptTemplate = """
        Eres un agente muy útil.

        #Histórico de mensajes
        {{$history}}

        #Nuevo mensaje
        User: {{$user_message}}

        Assistant:
    """;

var settings = new OpenAIPromptExecutionSettings() { Temperature = 0.7f, MaxTokens = 1000 };

var history = new List<Message>();

while (true)
{
    Console.Write("Prompt: ");
    var message = Console.ReadLine();

    var kernelArgs = new KernelArguments(settings)
    {
        { "history", history.AsString() },
        { "user_message", message }
    };

    history.Add(new Message("User", message));

    var result = await kernel.InvokePromptAsync(promptTemplate, kernelArgs);

    var resultContent = result.GetValue<string>();

    history.Add(new Message("Assistant", resultContent));
    
    Console.WriteLine($"\n{resultContent}\n");

}

static class HistoryExtensions
{
    public static string AsString(this IEnumerable<Message> history)
    {
        return string.Join("\n", history
               .TakeLast(10)
               .Select(t => $"{t.Role}: {t.Content.Replace("\n", " ").Trim()}"));
    }
}

public record Message(string Role, string Content);