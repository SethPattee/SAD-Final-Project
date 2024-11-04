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

        public void Receive(List<Product> products)
        {
            products.ForEach(p => { 
                if (!ProductInventory.Contains(p))
                {
                    ProductInventory.Add(p);
                }
                else
                {
                    ProductInventory.Find(x => x.ProductName == p.ProductName).Price = p.Price;
                }
            });
        }

        public void ShipOrder(List<Product> products)
        {
            List<Product> OrderLine = new List<Product>();

            products.ForEach(p => { 
                if (ProductInventory.Contains(p))
                {
                    OrderLine.Add(p);
                }
                else
                {
                    p.Price = 0;
                    p.Quantity = -1;
                    OrderLine.Add(p);
                }
            });
        }

        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
