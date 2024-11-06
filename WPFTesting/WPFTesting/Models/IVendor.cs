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
    void Receive(List<Product> products)
    {
        foreach (var product in products)
        {
            var inventoryProduct = ProductInventory.Find(p => p.ProductName == product.ProductName);
            if (inventoryProduct != null)
            {
                inventoryProduct.Quantity += product.Quantity;
            }
            else
            {
                ProductInventory.Add(new Product { ProductName = product.ProductName, Quantity = product.Quantity });
            }
        }
    }
    void Process();
    void ShipOrder(List<Product> products)
    {
        foreach (var product in products)
        {
            var inventoryProduct = ProductInventory.Find(p => p.ProductName == product.ProductName);
            if (inventoryProduct != null)
            {
                if (inventoryProduct.Quantity >= product.Quantity)
                {
                    inventoryProduct.Quantity -= product.Quantity;
                }
                else
                {
                    Console.WriteLine($"Not enough {product.ProductName} in inventory to ship.");
                }
            }
        }
    }
}
