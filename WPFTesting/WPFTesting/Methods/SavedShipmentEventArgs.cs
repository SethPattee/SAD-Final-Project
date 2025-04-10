using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FactorySADEfficiencyOptimizer.Models;

namespace FactorySADEfficiencyOptimizer.Methods
{
    public class SavedShipmentEventArgs : EventArgs
    {
        public required ObservableCollection<Shipment> SavedShipmentList;
    }
}
