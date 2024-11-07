using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Documents;
using WPFTesting.Data;
using WPFTesting.Models;
using WPFTesting.Shapes;
using YourNamespace;

namespace WPFTesting.ViewModel;

public class SupplyChainViewModel : INotifyPropertyChanged
{
    private IInitializedDataProvider _boxProvider;
    public event PropertyChangedEventHandler? PropertyChanged;
    private ObservableCollection<SupplierUIValues> _supplierList = new ObservableCollection<SupplierUIValues>();
    private List<Shipment> _shipmentList = new List<Shipment>();
    private ObservableCollection<EndpointUIValues> _endpointList = new ObservableCollection<EndpointUIValues>();
    public List<Shipment> ShipmentList
    {
        get { return _shipmentList; }
        set
        {
            _shipmentList = value;
            OnPropertyChanged(nameof(ShipmentList));
        }
    }

    public string ShortestPath;

    public SupplyChainViewModel(IInitializedDataProvider boxProvider)
    {
        _boxProvider = boxProvider;
    }

    public ObservableCollection<SupplierUIValues> SupplierList
    {
        get => _supplierList;
        set
        {
            _supplierList = value;
            OnPropertyChanged(nameof(SupplierList));
        }
    }

    protected void OnPropertyChanged(string name = null)
    {
        if (name is not null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }

    public void updateFileSave()
    {
        _boxProvider.SaveSupplierInfoAsync(SupplierList);
        //save the Shipping line here
    }


    public void Load()
    {
        //if (Boxes.Any()) return; //don't override any existing data

        var boxes = _boxProvider.GetBoxValuesAsync();
        if (boxes is not null)
        {
            foreach (var box in boxes)
            {
                SupplierList.Add(box);
            }
        }

    }
    public void AddEndpointToChain(EndpointUIValues endpoint)
    {
        SupplierList.Add(endpoint);
        OnPropertyChanged(nameof(SupplierList));
    }

    public void AddSupplierToChain(SupplierUIValues supplier)
    {
        SupplierList.Add(supplier);
        OnPropertyChanged(nameof(SupplierList));
    }

    public void AdvanceTime()
    {
        foreach (Shipment delivery in ShipmentList)
        {
            //shipOrder
            delivery.Sender.ShipOrder(delivery.Products);
            //Receive 
            delivery.Receiver.Receive(delivery.Products);
        }
        foreach (var v in _supplierList)
        {
            v.supplier.Process();
        }
    }

    public Shipment ShipmentFirstSuplier(Supplier target)
    {
        Shipment targetShipment = new Shipment();
        targetShipment.Sender = target;
        targetShipment.Products.Clear();
        Product toShip = target.ProductInventory.First();
        targetShipment.Products.Add(new Product());
        targetShipment.Products[0].ProductName = toShip.ProductName;
        targetShipment.Products[0].Quantity = 5;
        return targetShipment;
    }
}
