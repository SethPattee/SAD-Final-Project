using FactorySADEfficiencyOptimizer.Models;

namespace FactorySADEfficiencyOptimizer.ViewShared
{
    public class BoxPropertiesBase
    {
        public int ConnectedBoxes { get; set; }
        public List<Product> Items { get; set; } = new List<Product>();
        public string Title { get; set; } = "";
    }
}