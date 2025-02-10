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
using WPFTesting.Models;

namespace FactorSADEfficiencyOptimizer.UIComponent
{
    /// <summary>
    /// Interaction logic for ShipmentsScheduled.xaml
    /// </summary>
    public partial class ShipmentsScheduled : Window
    {
        private ObservableCollection<Shipment> _testshipments;
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
        AnalizorModel ShipmentModel { get; set; }
        public EventHandler? SaveShipmentDetails;
        public Product product { get; set; }
        public ObservableCollection<Product> items { get; set; }

        public ShipmentsScheduled()
        {
            InitializeComponent();
        }
        public ShipmentsScheduled(AnalizorModel s)
        {
            InitializeComponent();
            ShipmentModel = s;
            Testshipments = new ObservableCollection<Shipment>()
            {
                new Shipment()
                {
                    IsRecurring = false,
                    Products = new ObservableCollection<Product>()
                    {
                        new Product()
                        {
                            ProductName = "highly marketable waluigi plush",
                            Quantity = 1,
                            Price = 100000
                        },
                        new Product()
                        {
                            ProductName = "highly marketable biohazard dude",
                            Quantity = 1,
                            Price = 10
                        }
                    }
                },
                new Shipment()
                {
                    IsRecurring = true,
                    Products = new ObservableCollection<Product>()
                    {
                        new Product()
                        {
                            ProductName = "highly marketable waluigi plush",
                            Quantity = 1,
                            Price = 100000
                        },
                        new Product()
                        {
                            ProductName = "highly marketable biohazard dude",
                            Quantity = 1,
                            Price = 10
                        }
                    }
                }
            };

            
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
                //ShipmentsList.Remove(button.DataContext as Shipment);
            }
        }

        public void DeleteComponentToShipment_Click(object? sender, RoutedEventArgs? e)
        {

        }

        private void SaveButton_Click(object? sender, RoutedEventArgs? e)
        {
            SavedShipmentEventArgs ssea = new SavedShipmentEventArgs()
            {
                SavedShipmentList = ShipmentModel.ShipmentList
            };
            SaveShipmentDetails?.Invoke(sender, ssea);
        }


        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        private void ProductListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is ListViewItem lvi)
            {
                product = ((Product)lvi.DataContext);
            }
        }

        private void ShipmentWindowList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(sender is ListViewItem lvi)
                items = ((Shipment)lvi.DataContext).Products;
        }
    }
}
