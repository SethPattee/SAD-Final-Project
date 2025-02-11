using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPFTesting.Models;

namespace FactorSADEfficiencyOptimizer.Methods
{
    public class SavedShipmentEventArgs : EventArgs
    {
        public required ObservableCollection<Shipment> SavedShipmentList;
    }
}
