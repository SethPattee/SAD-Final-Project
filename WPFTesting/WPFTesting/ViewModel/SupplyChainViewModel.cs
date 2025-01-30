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
    public int Duration { get; set; }

    private IInitializedDataProvider _boxProvider;
    public event PropertyChangedEventHandler? PropertyChanged;
    private ObservableCollection<EndpointUIValues> _endpointList;
    public ObservableCollection<EndpointUIValues> EndpointList
    {
        get => _endpointList;
        set { 
            _endpointList = value;
            OnPropertyChanged(nameof(EndpointList));
        }
    }
    private EndpointUIValues _selectedEndpoint;
    public EndpointUIValues? SelectedEndpoint
    {
        get => _selectedEndpoint;
        set
        {
            _selectedEndpoint = value ?? new EndpointUIValues();
            OnPropertyChanged(nameof(SelectedEndpoint));
        }
    }
    private ProductLine _selectedComponentLine;
    public ProductLine SelectedComponentLine
    {
        get => _selectedComponentLine;
        set
        {
            _selectedComponentLine = value;
            OnPropertyChanged(nameof(SelectedComponentLine));
        }
    }

    private SupplierUIValues _selectedSupplier;
    public SupplierUIValues? SelectedSupplier
    {
        get => _selectedSupplier;
        set
        {
            _selectedSupplier = value;
            OnPropertyChanged(nameof(SelectedSupplier));
        }
    }
    private Shipment _selectedShipment;
    public Shipment? SelectedShipment
    {
        get => _selectedShipment;
        set
        {
            _selectedShipment = value;
            OnPropertyChanged(nameof(SelectedShipment));
        }
    }

    private List<Shipment> _shipmentList = new List<Shipment>();
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
        EndpointList = new ObservableCollection<EndpointUIValues>();
    }

    private ObservableCollection<SupplierUIValues> _supplierList = new ObservableCollection<SupplierUIValues>();
    public ObservableCollection<SupplierUIValues> SupplierList
    {
        get => _supplierList;
        set
        {
            _supplierList = value;
            OnPropertyChanged(nameof(SupplierList));
        }
    }

    protected void OnPropertyChanged(string? name = null)
    {
        if (name is not null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }

    public void updateFileSave()
    {
        List<SupplierUIValues> endpointsAndSuppliers = new List<SupplierUIValues>();
        endpointsAndSuppliers.AddRange(SupplierList);
        endpointsAndSuppliers.AddRange(EndpointList);
        _boxProvider.SaveSupplierInfoAsync(endpointsAndSuppliers);
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
                if (box is EndpointUIValues Endp)
                {
                    EndpointList.Add(Endp);
                }
                else if (box is SupplierUIValues supp)
                {
                    SupplierList.Add(supp);
                }
            }
        }

    }
    public void AddEndpointToChain(EndpointUIValues endpoint)
    {
        EndpointList.Add(endpoint);
        OnPropertyChanged(nameof(EndpointList));
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
            var sentProds = delivery.Sender.ShipOrder(delivery.Products);
            List<Product> listSent = new List<Product>();
            foreach (var prod in sentProds)
            {
                listSent.Add(prod);
            }
            //Receive 
            delivery.Receiver.Receive(listSent);
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

    public void PassTimeUntilDuration()
    {
        for (int i = 0; i < Duration; i++)
        {
            AdvanceTime();
        }
    }
}
