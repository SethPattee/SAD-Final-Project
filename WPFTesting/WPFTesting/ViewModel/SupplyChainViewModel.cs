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
    private IInitializedDataProvider _boxProvider;
    public event PropertyChangedEventHandler? PropertyChanged;
    private ObservableCollection<SupplierUIValues> _supplierList = new ObservableCollection<SupplierUIValues>();
    public List<Shippment> ShipmentList = new List<Shippment>();
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
        if(name is not null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }

    public void updateFileSave()
    {
        _boxProvider.SaveSupplierInfoAsync(SupplierList);
    }


    public async Task LoadAsync()
    {
        //if (Boxes.Any()) return; //don't override any existing data

        var boxes = await _boxProvider.GetBoxValuesAsync();
        if (boxes is not null)
        {
            foreach (var box in boxes)
            {
                SupplierList.Add(box);
            }
        }

    }

    public void AddSupplierToChain(SupplierUIValues supplier)
    {
        SupplierList.Add(supplier);
        OnPropertyChanged(nameof(SupplierList));
    }



}
