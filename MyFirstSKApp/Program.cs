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
                     .AddFromType<UserProfilePlugin>()
             .Services.AddLogging(logging =>
             {
                 logging.AddConsole();
                 logging.SetMinimumLevel(LogLevel.Trace);
             }).AddSingleton<IFunctionInvocationFilter, MyFunctionInvocationFilter>();

var kernel = kernelBuilder.Build();

var promptTemplate = """
        Eres un agente muy útil.

        #Usuario actual
        {{UserProfilePlugin.GetUserProfile $user_id}}

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
        { "history", history.AsString() },
        { "user_id", 25 }
    };

    history.Add(new Message("User", message));

    var result = await kernel.InvokePromptAsync(promptTemplate,
                                                kernelArgs);

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


public class ProcessPlugin
{
    [KernelFunction]
    [Description("Regresa la información de todos los procesos de la máquina.")]
    public IEnumerable<string> GetProcesses()
    {
        return Process.GetProcesses().Select(p => $"Id:{p.Id} ProcessName:{p.ProcessName}");
    }
}

public class UserProfilePlugin
{
    [KernelFunction]
    [Description("Regresa el perfil completo del usuario especificado.")]
    public UserProfile GetUserProfile([Description("El identificador único del usuario.")] int id)
    {
        return new UserProfile(id, "Rodrigo Díaz Concha", "SK Course Authors");
    }
}

public record UserProfile(int Id, string FullName, string Department)
{
    public override string ToString()
    {
        return $"Id: {Id}\n Nombre completo: {FullName}\n Departamento: {Department}\n";
    }
}

public class MyFunctionInvocationFilter : IFunctionInvocationFilter
{
    public Task OnFunctionInvocationAsync(FunctionInvocationContext context, 
                                          Func<FunctionInvocationContext, Task> next)
    {
        return next(context);
    }
}