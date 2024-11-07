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
    private Guid _id;
    private List<Product> _productInventory = new();
    private List<Product> _componentInventory = new();
    private List<ComponentToProductTransformer> _productionList = new();
    private List<Product> _deliveryrequirementslist = new();
    private decimal _profit;
    public EndpointNode()
    {
        _profit = (decimal)1000.0;
    }
    public string Name { 
        get => _name; 
        set { 
            _name = value;
            OnPropertyChanged(nameof(Name));
        } }
    public Guid Id 
        { 
            get => _id; 
            set { 
                _id = value; 
                OnPropertyChanged(nameof(Id));
        } }
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
    public List<ComponentToProductTransformer> ProductionList
    {
        get => _productionList;
        set
        {
            _productionList = value;
            OnPropertyChanged(nameof(ProductionList));
        }
    }
    public List<Product> DeliveryRequirementsList
    {
        get => _deliveryrequirementslist;
        set
        {
            _deliveryrequirementslist = value;
            OnPropertyChanged(nameof(DeliveryRequirementsList));
        }
    }
    public decimal Profit
    {
        get => _profit;
        set
        {
            _profit = value;
            OnPropertyChanged(nameof(Profit));
        }
    }


    public void ProduceProduct()
    {
        _productionList.ForEach(pl =>
        {
            List<(int,float)> ComponentIndices = new List<(int,float)>();
            pl.Components.ForEach(component =>
            {
                ComponentIndices.Add((_componentInventory.FindIndex(x => component.ProductName == x.ProductName && x.Quantity >= component.Quantity),
                                    component.Quantity));
            });
            ComponentIndices.RemoveAll(i => i.Item1 < 0);
            if (ComponentIndices.Count == pl.Components.Count)
            {
                ComponentIndices.ForEach(index => _componentInventory[index.Item1].Quantity -= index.Item2);
                ProductInventory.FirstOrDefault(pibin => pibin.ProductName == pl.Product.ProductName).Quantity += pl.Product.Quantity;
            }
        });

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

    public void Receive(List<Product> products)
    {
        products.ForEach(p =>
        {
            ComponentInventory.ForEach(component =>
            {
                if (component.ProductName == p.ProductName)
                {
                    component.Quantity += p.Quantity;
                }
                else
                {
                    ComponentInventory.Add(p);
                }
            });
        });
    }

    public List<Product> ShipOrder(List<Product> p)
    {
        p.ForEach(pitem =>
        {
            var targetindex = ProductInventory.FindIndex(x => x.ProductName == pitem.ProductName);
            if(ProductInventory[targetindex].Quantity - pitem.Quantity >= 0) {
                ProductInventory[targetindex].Quantity -= pitem.Quantity;
                Profit += (decimal)pitem.Quantity * ProductInventory[targetindex].Price;
            } 
            else {
                Profit += (decimal)ProductInventory[targetindex].Quantity * ProductInventory[targetindex].Price;
                ProductInventory[targetindex].Quantity = 0;
            }
        });

        return _deliveryrequirementslist;
    }

    public void Process()
    {
        ProduceProduct();
    }
}
