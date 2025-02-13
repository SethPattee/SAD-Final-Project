using FactorSADEfficiencyOptimizer.Methods;
using FactorSADEfficiencyOptimizer.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using FactorySADEfficiencyOptimizer.Models;

namespace FactorSADEfficiencyOptimizer.UIComponent;

/// <summary>
/// Interaction logic for ShipmentsScheduled.xaml
/// </summary>
public partial class ShipmentsScheduled : Window
{
    private ObservableCollection<Shipment> _testshipments;
    public Shipment SelectedShipment { get; set; }
    public ObservableCollection<Shipment> Testshipments
    {
        get
        {
            return _testshipments;
        }
        set
        {
            _testshipments = value;
            OnPropertyChanged(nameof(Testshipments));
        }
    }
    public EventHandler? SaveShipmentDetails;

    private Product _productitem;
    public Product ProductItem {
        get
        {
            return _productitem;
        }
        set
        {
            _productitem = value;
            OnPropertyChanged(nameof(ProductItem));
        }
    }

    private ObservableCollection<Supplier> _suppliers;
    public ObservableCollection<Supplier> Suppliers
    {
        get
        {
            return _suppliers;
        }
        set
        {
            _suppliers = value;
            OnPropertyChanged(nameof(Suppliers));
        }
    }
    private ObservableCollection<Product> _productitems;
    public ObservableCollection<Product> ProductItems { get {
            return _productitems;
        }
        set {
            _productitems = value;
            OnPropertyChanged(nameof(ProductItems));
        }
    }

    public ShipmentsScheduled(AnalizorModel simModel)
    {
        InitializeComponent();
        ScheduledShipmentsList.DataContext = this;
        SelectedProductList.DataContext = this;
        Testshipments = simModel.ShipmentList;
    }

    public void AddNewShipment_Click(object? sender, RoutedEventArgs? e)
    {
        Testshipments.Add(new Shipment());
        OnPropertyChanged(nameof(Testshipments));
        //Shipment s = new();
        
        //ShipmentsList.Add(s);
        //OnPropertyChanged(nameof(ShipmentsList));
    }

    public void AddComponentToShipment_Click(object? sender, RoutedEventArgs? e)
    {
        //Product p = new();
        //selected_shipment.Products.Add(p);
        //OnPropertyChanged(nameof(ShipmentsList));
    }

    public void DeleteShipment_Click(object? sender, RoutedEventArgs? e)
    {
        if(sender is Button button)
        {
            Testshipments.Remove(button.DataContext as Shipment);
        }
    }

    public void DeleteComponentToShipment_Click(object? sender, RoutedEventArgs? e)
    {

    }

    private void SaveButton_Click(object? sender, RoutedEventArgs? e)
    {
        SavedShipmentEventArgs ssea = new SavedShipmentEventArgs()
        {
            SavedShipmentList = Testshipments
        };
        SaveShipmentDetails?.Invoke(sender, ssea);
    }


    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged(string name)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }


    private void ShipmentWindowList_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        ProductItems = SelectedShipment.Products;

        SelectedProductList.ItemsSource = ProductItems;
    }
}