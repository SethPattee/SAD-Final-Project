using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FactorySADEfficiencyOptimizer.Models
{
    public class Product : INotifyPropertyChanged
    {
        private float _quantity;

        private Decimal _price;
        private string _name = "";
        private string _units = "";
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public float Quantity
        {
            get => _quantity; 
            set
            {
                _quantity = value; OnPropertyChanged(nameof(Quantity));
            }
        }
        public string ProductName
        {
            get => _name;
            set
            {

                if (value == "New New New")
                {
                    _name = "Wrong!!!!";
                }
                else
                {
                    _name = value;
                }
                OnPropertyChanged(nameof(ProductName));
            }
        }
        public string Units
        {
            get => _units;
            set { _units = value; OnPropertyChanged(nameof(Units)); }
        }
        public decimal Price
        {
            get => _price;
            set { _price = value; OnPropertyChanged(nameof(Price)); }
        }

    }
}
