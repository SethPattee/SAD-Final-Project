using FactorySADEfficiencyOptimizer.Methods;
using FactorySADEfficiencyOptimizer.ViewModel;
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
using FactorySADEfficiencyOptimizer.UIComponent.EventArguments;
using FactorySADEfficiencyOptimizer.UIComponent;

namespace FactorySADEfficiencyOptimizer.UIComponent;

/// <summary>
/// Interaction logic for ShipmentsScheduled.xaml
/// </summary>
public partial class ShipmentsScheduled : Window
{
    public ShipmentsScheduled(AnalizorModel simModel)
    {
        InitializeComponent();
        ScheduledShipmentsList.DataContext = this;
        SelectedProductList.DataContext = this;
        Testshipments = new ObservableCollection<Shipment>(simModel.ShipmentList);

    }
    private bool IsCancellingCloseRequest = true;
    ConfirmCloseScheduledShipmentDialogue? ConfirmPopup = null;
    protected override void OnClosing(CancelEventArgs e)
    {
        if (IsCancellingCloseRequest)
        {
            ConfirmPopup = new ConfirmCloseScheduledShipmentDialogue();
            ConfirmPopup.DialogueConfirmed += DialogueConfirmedByUser;
            ConfirmPopup.Owner = this;
            ConfirmPopup.Show();
        }

        e.Cancel = IsCancellingCloseRequest;
        base.OnClosing(e);
    }

    private void DialogueConfirmedByUser(object? sender, EventArgs e)
    {
        if(e is CloseScheduledShipmentsEventArgs cssea)
        {
            if (cssea.UserSaveChoice)
            {
                SaveButton_Click(sender, cssea);
            }
            IsCancellingCloseRequest = false;
            Close();
        }
    }

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


    public void AddNewShipment_Click(object? sender, RoutedEventArgs? e)
    {
        Testshipments.Add(GetDefaultShipment());
        OnPropertyChanged(nameof(Testshipments));
        //Shipment s = new();
        
        //ShipmentsList.Add(s);
        //OnPropertyChanged(nameof(ShipmentsList));
    }

    public Product GetDefaultProduct()
    {
        Product newprod = new()
        {
            ProductName = "new product"
        };

        return newprod;
    }

    public Shipment GetDefaultShipment()
    {
        Shipment newprod = new()
        {
            Sender = new Supplier()
            {
                Name = "new sender"
            },
            Receiver = new Supplier()
            {
                Name = "new receiver"
            }
        };

        return newprod;
    }

    public void AddComponentToShipment_Click(object? sender, RoutedEventArgs? e)
    {
        if(SelectedShipment is not null)
        {
            Product p = GetDefaultProduct();
            SelectedShipment.Products.Add(p);
        }
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
        if(sender is Button button)
        {
            ProductItems.Remove(button.DataContext as Product);
        }
    }

    private void SaveButton_Click(object? sender, RoutedEventArgs? e)
    {
        SavedShipmentEventArgs ssea = new SavedShipmentEventArgs()
        {
            SavedShipmentList = Testshipments
        };

        SaveShipmentDetails?.Invoke(sender, ssea);
        IsCancellingCloseRequest = false;
        this.Close();
    }


    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged(string name)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }


    private void ShipmentWindowList_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if(sender is ListView lvi){
            AssignProductList((Shipment)lvi.SelectedItem);
        }
    }
    private void ShowProductListButton_Click(object sender, RoutedEventArgs e)
    {
        if(sender is Button b)
        {
            AssignProductList((Shipment)b.DataContext);
        }
    }

    public event EventHandler? SelectShipmentLineHandler;
    private void SelectLineFromWindow_MouseDown(object sender, MouseButtonEventArgs e)
    {
        if (sender is ListView b)
        {
            var shipmentSelectedEvent = new SelectedShipmentWindowMouseButtonEventArgs(e.MouseDevice, e.Timestamp, e.ChangedButton);
            shipmentSelectedEvent.AssignSelectedShipment((Shipment)b.Items.CurrentItem);
            SelectShipmentLineHandler?.Invoke(this, shipmentSelectedEvent);
        }
    }

    private void AssignProductList(Shipment SelectedShipment)
    {
        ProductItems = SelectedShipment.Products;
        SelectedProductList.ItemsSource = ProductItems;
    }

   

}