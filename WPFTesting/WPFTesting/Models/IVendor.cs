using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFTesting.Models;

public interface IVendor
{
    string Name { get; set; }
    Guid Id { get; set; }
    List<Product> ProductInventory { get; set; }
    void Receive(List<Product> products);
    void Process();
    List<Product> ShipOrder(List<Product> products);
}
