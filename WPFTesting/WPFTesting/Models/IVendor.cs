using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFTesting.Models;

public interface IVendor
{
    string Name { get; set; }
    Guid Id { get; set; }
    ObservableCollection<Product> ProductInventory { get; set; }
    ObservableCollection<Product> ComponentInventory { get; set; }
    void Receive(List<Product> products);
    void Process();
    ObservableCollection<Product> ShipOrder(List<Product> products);
}
