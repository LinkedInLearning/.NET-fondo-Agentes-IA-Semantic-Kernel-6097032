using FabuRobotics.AIAgent.Plugins;
using FabuRobotics.AIAgent.Services;
using Microsoft.SemanticKernel;
var builder = WebApplication.CreateBuilder(args);

var apiKey = builder.Configuration.GetValue<string>("FabuRobotics:ApiKey");
var modelId = builder.Configuration.GetValue<string>("FabuRobotics:ModelId");

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddKernel()
                .AddOpenAIChatCompletion(modelId!, apiKey!)
                .Plugins
                    .AddFromType<InventoryPlugin>()
                    .AddFromType<ProductionPlugin>();
builder.Services.AddScoped<AgentService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();