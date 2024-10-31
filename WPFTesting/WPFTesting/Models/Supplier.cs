using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFTesting.Models
{
    public class Supplier : INotifyPropertyChanged, IVendor
    {
        private string _name ="";
        public Guid Id { get; set; }
        public string Name {

            get => _name;
            set
            {
                _name = value;
                OnPropertyChanged(nameof(_name));
            }
        
        }
        public List<Product> ProductInventory { get; set; } = new List<Product>();

        public event PropertyChangedEventHandler? PropertyChanged;

        public void Process()
        {
            // nothing yet
        }

        public void Receive(List<Product> prducts)
        {
            //add to our products by the quantity in incoming products
        }

        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
