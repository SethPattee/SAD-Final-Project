using FactorSADEfficiencyOptimizer.ViewModel;
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
        _boxProvider.SaveSupplierInfo(endpointsAndSuppliers);
        _boxProvider.SaveShipmentInfo(ShipmentList);
    }


    public void Load()
    {
        var boxes = _boxProvider.GetBoxValues();
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
        IEnumerable<Shipment> ship = _boxProvider.GetShipments(EndpointList, SupplierList);
        if (ship is not null)
        {
            ShipmentList.AddRange(ship);
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
        ToolsForViewModel.AdvanceTimeforShipmentList(ShipmentList);
        ToolsForViewModel.AdvancetimeForSupplierList(SupplierList);
        ToolsForViewModel.AdvanceTimeForEndpointList(EndpointList);
    }


    public void PassTimeUntilDuration()
    {
        for (int i = 0; i < Duration; i++)
        {
            AdvanceTime();
        }
    }
}
