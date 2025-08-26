using Microsoft.SemanticKernel;
using System.ComponentModel;

namespace FabuRobotics.AIAgent.Plugins;

public class ProductionPlugin
{
    [KernelFunction]
    [Description("Crea una nueva orden de ensamblaje.")]
    public async Task<OrderCreated> CreateOrder()
    {
        throw new NotImplementedException();
    }

    [KernelFunction]
    [Description("Inicia el proceso de ensamblaje de la orden.")]
    public async Task StartAssembly()
    {
        throw new NotImplementedException();
    }

    [KernelFunction]
    [Description("Obtiene el estatus de la orden especificada.")]
    public string GetOrderStatus()
    {
        throw new NotImplementedException();
    }
}

public record OrderCreated(Guid OrderId,
                           string CustomerId,
                           string Sku,
                           int Quantity,
                           DateTime OrderDate);