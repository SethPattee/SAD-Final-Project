using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFTesting.Models;

public class EndpointNode : IVendor, INotifyPropertyChanged
{
    private string _name = "";
    private List<Product> _productInventory = new();
    private List<Product> _componentInventory = new();
    public EndpointNode()
    {
        
    }
    public string Name { 
        get => _name; 
        set { 
            _name = value;
            OnPropertyChanged(nameof(Name));
        } }
    public Guid Id { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public List<Product> ProductInventory {
        get => _productInventory;
        set
        {
            _productInventory = value;
            OnPropertyChanged(nameof(ProductInventory));
        }
    }
    public List<Product> ComponentInventory {
        get => _componentInventory;
        set
        {
            _componentInventory = value;
            OnPropertyChanged(nameof(ComponentInventory));
        }
    }

    public void ProduceProduct()
    {
        foreach (var componet in _componentInventory)
        {
            componet.Quantity = componet.Quantity - 1;   
        }
        OnPropertyChanged(nameof(ComponentInventory));
        
        foreach (var product in _productInventory)
        {
            product.Quantity = product.Quantity + 1; 
        }
        OnPropertyChanged(nameof(ProductInventory));
    }


    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public void Receive(List<Product> prducts)
    {
        //add to our products by the quantity in incoming products
    }

    public void Process()
    {
        ProduceProduct();
    }
}
