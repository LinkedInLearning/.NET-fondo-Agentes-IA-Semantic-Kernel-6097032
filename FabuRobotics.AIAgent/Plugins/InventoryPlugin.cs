using Microsoft.SemanticKernel;
using System.ComponentModel;

namespace FabuRobotics.AIAgent.Plugins;

public class InventoryPlugin
{
    private readonly Dictionary<string, bool> availability = new()
    {
        { "FB-01", true },
        { "FB-02", true },
        { "FB-03", false }
    };

    [KernelFunction]
    [Description("Indica si existen en inventario todos los componentes necesarios para ensamblar el SKU.")]
    public bool GetAvailabilityForSku([Description("El SKU del producto que se desea ensamblar.")] string sku)
    {
        return availability.First(availability => availability.Key.Equals(sku, StringComparison.OrdinalIgnoreCase)).Value;
    }
}