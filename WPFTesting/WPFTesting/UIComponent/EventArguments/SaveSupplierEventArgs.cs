using FactorySADEfficiencyOptimizer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FactorySADEfficiencyOptimizer.UIComponent.EventArguments
{
    class SaveSupplierEventArgs : EventArgs
    {
        public Supplier? supplier { get; set; }
    }
}
