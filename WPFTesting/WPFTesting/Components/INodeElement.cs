using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FactorySADEfficiencyOptimizer.Shapes;

namespace FactorySADEfficiencyOptimizer.Components
{
    public interface INodeElement
    {
        SupplierUIValues NodeUIValues { get; set; }
    }
}
