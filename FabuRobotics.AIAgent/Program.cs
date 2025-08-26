using FabuRobotics.AIAgent.Plugins;
using FabuRobotics.AIAgent.Services;
using Microsoft.KernelMemory;
using Microsoft.SemanticKernel;
var builder = WebApplication.CreateBuilder(args);

var apiKey = builder.Configuration.GetValue<string>("FabuRobotics:ApiKey");
var modelId = builder.Configuration.GetValue<string>("FabuRobotics:ModelId");
var qdrantUrl = "http://localhost:6333";
builder.Services.AddControllers();
builder.Services.AddOpenApi();

builder.Services.AddKernelMemory<MemoryServerless>(kernelBuilder =>
{
    kernelBuilder.WithOpenAIDefaults(apiKey!)
                 .WithQdrantMemoryDb(qdrantUrl);
}, new KernelMemoryBuilderBuildOptions()
{
    AllowMixingVolatileAndPersistentData = true
});

builder.Services.AddKernel()
                .AddOpenAIChatCompletion(modelId!, apiKey!)
                .Plugins
                    .AddFromType<InventoryPlugin>()
                    .AddFromType<ProductionPlugin>()
                    .AddFromType<MemoryPlugin>();
builder.Services.AddScoped<AgentService>();
builder.Services.AddMemoryCache();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

if (bool.TryParse(Environment.GetEnvironmentVariable("VectorizeAtStartup"), out bool vectorize) && vectorize)
{
    using (var scope = app.Services.CreateScope())
    {
        try
        {
            var kernelMemory = scope.ServiceProvider.GetRequiredService<IKernelMemory>();
            await kernelMemory.ImportWebPageAsync("https://faburobotics.com");
            await kernelMemory.ImportDocumentAsync("FB-02_Fact_Sheet_Spanish.pdf", "fb-02");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"El proceso de importación ha fallado: {ex.Message}");
        }
    }
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();