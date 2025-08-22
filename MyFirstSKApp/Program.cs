using Microsoft.Extensions.AI;
using Microsoft.SemanticKernel;

var modelId = "gpt-4o";
var apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY");
var kernelBuilder = Kernel.CreateBuilder();

kernelBuilder.AddOpenAIChatClient(modelId, apiKey);

var kernel = kernelBuilder.Build();

var chatClient = kernel.GetRequiredService<IChatClient>();

var prompt = """
    Cuáles son las ciudades más grandes del mundo?
    Regresa un arreglo en JSON, donde cada elemento tiene
    la siguiente estructura: Position, Name, Country, Population
    """;

var chatOptions = new ChatOptions()
{
    Temperature = 0.1f,
    ResponseFormat = ChatResponseFormat.Json
};

var response = await chatClient.GetResponseAsync<IEnumerable<City>>(prompt, chatOptions);

foreach (var city in response.Result)
{
    Console.WriteLine(city);
}

public record City(int Position, string Name, string Country, string Population);