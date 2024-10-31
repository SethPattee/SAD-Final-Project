using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using WPFTesting.Models;

namespace WPFTesting.Shapes
{
    public class EndpointUIValues : SupplierUIValues
    {
        private decimal? _profit;
        public decimal? Profit
        {
            get => _profit;
            set {
                _profit = value;
                OnPropertyChanged(nameof(Profit));
            }
        }
        public EndpointUIValues()
        {
            supplier = new EndpointNode();
        }

    }
}
