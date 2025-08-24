using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Microsoft.SemanticKernel.PromptTemplates.Handlebars;
using Microsoft.SemanticKernel.PromptTemplates.Liquid;

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
        <message role="system">
            Eres un agente muy útil.
        </message>

        {{#each history}}
        <message role="{{role}}">{{content}}</message>
        {{/each}}
    """;

var settings = new OpenAIPromptExecutionSettings() { Temperature = 0.7f, MaxTokens = 1000 };

var history = new List<Message>();

var factory = new HandlebarsPromptTemplateFactory();

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
                                                kernelArgs, 
                                                templateFormat:"handlebars",
                                                promptTemplateFactory: factory);

    var resultContent = result.GetValue<string>();

    history.Add(new Message("Assistant", resultContent));
    
    Console.WriteLine($"\n{resultContent}\n");

}

public record Message(string Role, string Content);