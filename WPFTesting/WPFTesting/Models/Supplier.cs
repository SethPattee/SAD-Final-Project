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
                if (ProductInventory.Find(x => x.ProductName == p.ProductName).ProductName == p.ProductName)
                {
                   
                    ProductInventory.Find(x => x.ProductName == p.ProductName).Quantity += p.Quantity;
                }
                else
                {
                    ProductInventory.Add(p);
                }
            });
        }

        public void ShipOrder(List<Product> products)
        {
            List<Product> OrderLine = new List<Product>();

            products.ForEach(p => { 
                if (ProductInventory.Find(x => x.ProductName == p.ProductName).ProductName == p.ProductName)
                {
                    OrderLine.Add(p);
                    ProductInventory.Find(x => x.ProductName == p.ProductName).Quantity -= p.Quantity;
                }
                else
                {
                    //p.Price = 0;
                    //p.Quantity = -1;
                    //OrderLine.Add(p);
                }
            });
        }

        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
