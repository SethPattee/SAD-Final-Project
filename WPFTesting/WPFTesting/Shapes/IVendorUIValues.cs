using System.ComponentModel;
using FactorySADEfficiencyOptimizer.Models;

namespace FactorySADEfficiencyOptimizer.Shapes
{
    public interface IVendorUIValues
    {
        int XPosition { get; set; }
        int YPosition { get; set; }
        IVendor supplier { get; set; }
        int XPosition1 { get; set; }
    }
}