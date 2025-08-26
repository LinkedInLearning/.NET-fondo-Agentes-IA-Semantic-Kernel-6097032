using Microsoft.Extensions.Caching.Memory;
using Microsoft.SemanticKernel;
using System.ComponentModel;

namespace FabuRobotics.AIAgent.Plugins;

public class ProductionPlugin(IMemoryCache memoryCache)
{
    [KernelFunction]
    [Description("Crea una nueva orden de ensamblaje.")]
    public async Task<OrderCreated> CreateOrder(
        [Description("El identificador del cliente.")] string customerId,
        [Description("El SKU del producto a ensamblar.")] string sku,
        [Description("La cantidad a ensamblar.")] int quantity = 1)
    {
        var orderId = Guid.NewGuid();
        var newOrderCreated = new OrderCreated(orderId,
                                               customerId,
                                               sku,
                                               quantity,
                                               DateTime.UtcNow);
        return newOrderCreated;
    }


    [KernelFunction]
    [Description("Inicia el proceso de ensamblaje de la orden.")]
    public async Task StartAssembly([Description("El identificador de la orden.")] Guid orderId)
    {
        System.Timers.Timer timer = new(10_000);
        OrderStatus currentStatus = OrderStatus.Preparing;

        timer.Elapsed += async (sender, e) =>
        {
            memoryCache.Set(orderId, currentStatus);

            (currentStatus, bool stopTimer) = currentStatus switch
            {
                OrderStatus.Preparing => (OrderStatus.Assembling, false),
                OrderStatus.Assembling => (OrderStatus.Assembled, false),
                OrderStatus.Assembled => (OrderStatus.Shipped, false),
                OrderStatus.Shipped => (currentStatus, true),
                _ => (currentStatus, false)
            };

            if (stopTimer)
            {
                timer.Stop();
            }
        };

        timer.Start();
    }


    [KernelFunction]
    [Description("Obtiene el estatus de la orden especificada.")]
    public string GetOrderStatus([Description("El identificador de la orden.")] Guid orderId)
    {
        var currentStatus = memoryCache.Get<OrderStatus>(orderId);
        return Enum.GetName(currentStatus)!;
    }

}

public record OrderCreated(Guid OrderId,
                           string CustomerId,
                           string Sku,
                           int Quantity,
                           DateTime OrderDate);

public enum OrderStatus
{
    Preparing,
    Assembling,
    Assembled,
    Shipped
}