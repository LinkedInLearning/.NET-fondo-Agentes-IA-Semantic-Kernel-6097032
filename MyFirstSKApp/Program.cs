using Microsoft.Extensions.AI;
using Microsoft.SemanticKernel;

var modelId = "gpt-4o";
var apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY");
var kernelBuilder = Kernel.CreateBuilder();

kernelBuilder.AddOpenAIChatClient(modelId, apiKey);

var kernel = kernelBuilder.Build();

var chatClient = kernel.GetRequiredService<IChatClient>();

var prompt = "Cuáles son las ciudades más grandes del mundo?";
