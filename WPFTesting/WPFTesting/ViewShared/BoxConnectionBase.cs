using System.Windows.Shapes;
using YourNamespace;

namespace FactorySADEfficiencyOptimizer.ViewShared
{
    public class BoxConnectionBase
    {
        public SupplierElement ConnectedBox { get; set; }
        public Line ConnectionLine { get; set; }
    }
}