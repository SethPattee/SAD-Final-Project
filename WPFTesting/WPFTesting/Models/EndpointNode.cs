using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace FactorySADEfficiencyOptimizer.Models;

public class EndpointNode : IVendor, INotifyPropertyChanged
{
    private ILogger logger;
    private string _name;
    private Guid _id;
    private ObservableCollection<Product> _productInventory;
    private ObservableCollection<Product> _componentInventory;
    private ObservableCollection<ProductLine> _productionList;
    private ObservableCollection<Product> _deliveryrequirementslist;
    private decimal _profit;
    public EndpointNode()
    {
        using ILoggerFactory loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
        logger = loggerFactory.CreateLogger("EndpointNode");
        _profit = (decimal)1234.0;
        _name = "";
        _productInventory = new ObservableCollection<Product>();
        _componentInventory = new ObservableCollection<Product>();
        _productionList = new ObservableCollection<ProductLine>();
        _deliveryrequirementslist = new ObservableCollection<Product>();
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
    public ObservableCollection<Product> ProductInventory {
        get => _productInventory;
        set
        {
            _productInventory = value;
            OnPropertyChanged(nameof(ProductInventory));
        }
    }
    public ObservableCollection<Product> ComponentInventory {
        get => _componentInventory;
        set
        {
            _componentInventory = value;
            OnPropertyChanged(nameof(ComponentInventory));
        }
    }
    public ObservableCollection<ProductLine> ProductionList
    {
        get => _productionList;
        set
        {
            _productionList = value;
            OnPropertyChanged(nameof(ProductionList));
        }
    }
    public ObservableCollection<Product> DeliveryRequirementsList
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
        //For every component set to make a given product:
        foreach( var pl in _productionList)
        {
            try
            {
                if (!pl.IsEnabled)
                    continue;

                bool CanProceed = false;
                var TransactionalComponentInventory = _componentInventory;
                if (_productInventory.Where(x => x.ProductName == pl.ResultingProduct.ProductName).Any())
                {
                    foreach (var component in pl.Components)
                    {
                        foreach(var inventorycomponent in TransactionalComponentInventory)
                        {
                            if(inventorycomponent.ProductName == component.ProductName
                                && inventorycomponent.Quantity >= component.Quantity)
                            {
                                inventorycomponent.Quantity -= component.Quantity;
                                CanProceed = true;
                                break;
                            }
                            else
                            {
                                CanProceed = false;
                                continue;
                            }
                        }
                        if (!CanProceed) break;
                    }

                    if (CanProceed)
                    {
                        _productInventory.Where(x => x.ProductName == pl.ResultingProduct.ProductName).First().Quantity += pl.ResultingProduct.Quantity;
                        _componentInventory = TransactionalComponentInventory;
                    }
                }
                else
                {
                    foreach (var component in pl.Components)
                    {
                        foreach (var inventorycomponent in TransactionalComponentInventory)
                        {
                            if (inventorycomponent.ProductName == component.ProductName
                                && inventorycomponent.Quantity >= component.Quantity)
                            {
                                inventorycomponent.Quantity -= component.Quantity;
                                CanProceed = true;
                                break;
                            }
                            else
                            {
                                CanProceed = false;
                                continue;
                            }
                        }
                        if (!CanProceed) break;
                    }

                    if (CanProceed)
                    {
                        _productInventory.Add(pl.ResultingProduct);

                        _componentInventory = TransactionalComponentInventory;
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogCritical(ex.Message);
            }

        };

        OnPropertyChanged(nameof(ComponentInventory));
        OnPropertyChanged(nameof(ProductInventory));
    }


    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public void Receive(List<Product> products)
    {
        foreach (var product in products)
        {
            if (ComponentInventory.FirstOrDefault(x => product is not null && x.ProductName == product.ProductName)?.ProductName == product.ProductName)
            {
                ComponentInventory.FirstOrDefault(x => x.ProductName == product.ProductName).Quantity += product.Quantity;
            }
            else
            {
                Product toAdd = new Product();
                toAdd.ProductName = product.ProductName;
                toAdd.Quantity = product.Quantity;
                ComponentInventory.Add(toAdd);
            }
            Profit -= product.Price;
        }
    }

    public ObservableCollection<Product> ShipOrder(List<Product> p)
    {

        p.ForEach(pitem =>
        {
            var targetindex = ProductInventory.ToList().FindIndex(x => x.ProductName == pitem.ProductName);
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
