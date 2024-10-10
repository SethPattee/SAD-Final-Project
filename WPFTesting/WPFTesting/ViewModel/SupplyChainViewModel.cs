using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using WPFTesting.Data;
using WPFTesting.Models;
using WPFTesting.Shapes;
using YourNamespace;

namespace WPFTesting.ViewModel;

public class SupplyChainViewModel : INotifyPropertyChanged
{
    private IBoxDataProvider _boxProvider;
    public event PropertyChangedEventHandler PropertyChanged;
    public ObservableCollection<BoxValues> SupplierList = new ObservableCollection<BoxValues>();
    public List<string> ShipmentList = new List<string>(); //Replace string with Shipment model when shipment model done

    public SupplyChainViewModel(IBoxDataProvider boxProvider)
    {
        _boxProvider = boxProvider;
    }

    protected void OnPropertyChanged(string name = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }


    public async Task LoadAsync()
    {
        //if (Boxes.Any()) return; //don't overide any existing data

        var boxes = await _boxProvider.GetBoxValuesAsync();
        if (boxes is not null)
        {
            foreach (var box in boxes)
            {
                SupplierList.Add(box);
            }
        }

    }

    public void AddSupplierToChain(BoxValues supplier)
    {
        SupplierList.Add(supplier);
        OnPropertyChanged("SupplierList");
    }

    public async Task CalculateSupplyChainOutputAsync()
    {
        
    }
}
