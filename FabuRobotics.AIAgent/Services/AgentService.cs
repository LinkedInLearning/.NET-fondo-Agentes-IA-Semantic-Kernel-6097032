using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.OpenAI;

namespace FabuRobotics.AIAgent.Services;

public class AgentService(Kernel kernel)
{
    private static OpenAIPromptExecutionSettings settings = new()
    {
        FunctionChoiceBehavior = FunctionChoiceBehavior.Auto(),
        ChatSystemPrompt = """
         Instrucciones obligatorias: responde siempre en español; 
             si el usuario saluda, comienza con un saludo cordial; 
             busca primero en tu memoria o base de conocimiento y, 
             si hay una respuesta completa y verificada, 
             úsala tal cual; si no existe, usa los plugins adecuados 
             y si estos devuelven una respuesta completa, 
             preséntala sin cambios; 
             si ni memoria ni plugins contienen la respuesta, 
             no generes una por tu cuenta; 
             sigue siempre el orden Memoria->Plugins y nunca combines fuentes.
             Instrucciones obligatorias: responde siempre en español; 
             si el usuario saluda, comienza con un saludo cordial; 
             busca primero en tu memoria o base de conocimiento y, 
             si hay una respuesta completa y verificada, 
             úsala tal cual; si no existe, usa los plugins adecuados 
             y si estos devuelven una respuesta completa, 
             preséntala sin cambios; 
             si ni memoria ni plugins contienen la respuesta, 
             no generes una por tu cuenta; 
             sigue siempre el orden Memoria->Plugins y nunca combines fuentes.
        """
    };

    private static KernelArguments arguments = new(settings);

    public async Task<string> Chat(string message)
    {
        var result = await kernel.InvokePromptAsync(message, arguments);
        return result.GetValue<string>();
    }
}