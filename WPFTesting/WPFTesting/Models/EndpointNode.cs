using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using FactorSADEfficiencyOptimizer.ViewModel;
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
    private ObservableCollection<DeliveryLine> _pastDLs;
    private ObservableCollection<DeliveryLine> _activeDLs;
    private ObservableCollection<Product> _daysUsedcomponents;
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
        _activeDLs = new ObservableCollection<DeliveryLine>();
        _pastDLs = new ObservableCollection<DeliveryLine>();
        _daysUsedcomponents = new ObservableCollection<Product>();
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
    public ObservableCollection<DeliveryLine> ActiveDeliveryLines
    {
        get => _activeDLs;
        set
        {
            _activeDLs = value;
            OnPropertyChanged(nameof(ActiveDeliveryLines));
        }
    }
    public ObservableCollection<DeliveryLine> PastDeliveryLines
    {
        get => _pastDLs;
        set
        {
            _pastDLs = value;
            OnPropertyChanged(nameof(PastDeliveryLines));
        }
    }
    public decimal Balance
    {
        get => _profit;
        set
        {
            _profit = value;
            OnPropertyChanged(nameof(Balance));
        }
    }
    public ObservableCollection<Product> DaysUsedComponents
    {
        get => _daysUsedcomponents;
    }

    public void ProduceProduct()
    {
        _daysUsedcomponents = new ObservableCollection<Product>();
        //For every component set to make a given product:
        foreach( var p in _productionList)
        {
            var pl = p.ShallowCopy();
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
                        CanProceed = true;
                        foreach(var inventorycomponent in TransactionalComponentInventory)
                        {
                            if(inventorycomponent.ProductName == component.ProductName
                                && inventorycomponent.Quantity >= component.Quantity)
                            {
                                inventorycomponent.Quantity -= component.Quantity;
                                saveUsedComponent(component);
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
                        var prodInventory = _productInventory.Where(x => x.ProductName == pl.ResultingProduct.ProductName).First();
                        prodInventory.Quantity = prodInventory.Quantity + pl.ResultingProduct.Quantity;
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
                                saveUsedComponent(component);
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

    private void saveUsedComponent(Product component)
    {
        var existingProd = _daysUsedcomponents.FirstOrDefault(p => p.ProductName == component.ProductName);
        if (existingProd != null)
        {
            existingProd.Quantity += component.Quantity;
        }
        else
        {
            Product usedComponent = component.ShallowCopy();
            _daysUsedcomponents.Add(usedComponent);
        }
        OnPropertyChanged(nameof(DaysUsedComponents));
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
            Balance -= product.Price * (decimal)product.Quantity;
        }
    }

    public ObservableCollection<Product> ShipOrder(List<Product> p)
    {
        ObservableCollection<Product> productsThatWereFulfilled = new();
        foreach(var pitem in p)
        {
            // If we aren't even delivering this type of item, ignore it and try the next one.
            if (!ActiveDeliveryLines.ToList().Exists(x => x.DeliveryItem.ProductName == pitem.ProductName))
                continue;

            var copyOfDeliverables = new ObservableCollection<DeliveryLine>(ActiveDeliveryLines);
            foreach (var i in copyOfDeliverables)
            {

                // see if there is an item of that name. If duplicate, use first.
                var item = ProductInventory.FirstOrDefault(x => x.ProductName == pitem.ProductName);

                // make sure item exists
                if (item is null)
                    continue;

                // We can only fulfill orders we have enough items for.
                if (item.Quantity >= i.DeliveryItem.Quantity)
                {
                    (ProductInventory.FirstOrDefault(x => x.ProductName == pitem.ProductName)
                    ?? throw new ArgumentOutOfRangeException("Could not find specified item."))
                    .Quantity -= i.DeliveryItem.Quantity;
                    
                    Balance += i.TotalPrice;
                    productsThatWereFulfilled.Add(pitem);

                    // If the delivery line is supposed to be fulfilled by this shipping action:
                    // Move the delivery line from active record to past record.
                    if (!i.IsRecurring)
                        i.IsFulfilled = true;

                    if (i.IsFulfilled)
                    {
                        ActiveDeliveryLines.Remove(i);
                        PastDeliveryLines.Add(i);
                    }
                }
            }
        }


        return productsThatWereFulfilled;
    }

    public void Process()
    {
        ProduceProduct();
    }
}
