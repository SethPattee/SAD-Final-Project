using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        public ObservableCollection<Product> ProductInventory { get; set; } = new ObservableCollection<Product>();

        public event PropertyChangedEventHandler? PropertyChanged;

        public void Process()
        {
            // nothing yet
        }

        public void Receive(List<Product> products)
        {
            products.ForEach(p => { 
                if (ProductInventory.FirstOrDefault(x => p is not null && x.ProductName == p.ProductName).ProductName == p.ProductName)
                {

                    ProductInventory.FirstOrDefault(x => x.ProductName == p.ProductName).Quantity += p.Quantity;
                }
                else
                {
                    ProductInventory.Add(p);
                }
            });
        }

        public ObservableCollection<Product> ShipOrder(List<Product> products)
        {
            ObservableCollection<Product> OrderLine = new ObservableCollection<Product>();

            products.ForEach(p => { 
                if (ProductInventory.FirstOrDefault(x => x.ProductName == p.ProductName)?.ProductName == p.ProductName)
                {
                    var targetIndex = ProductInventory.ToList().FindIndex(x => x.ProductName == p.ProductName);
                    if (ProductInventory[targetIndex].Quantity-p.Quantity < 0)
                    {
                        OrderLine.Add(ProductInventory[targetIndex]);
                        ProductInventory[targetIndex].Quantity = 0;
                    }
                    else
                    {
                        OrderLine.Add(p);
                        ProductInventory[targetIndex].Quantity -= p.Quantity;
                    }
                    
                }
                else
                {
                    //p.Price = 0;
                    //p.Quantity = -1;
                    //OrderLine.Add(p);
                }

            });
            
            return OrderLine;
        }



        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
