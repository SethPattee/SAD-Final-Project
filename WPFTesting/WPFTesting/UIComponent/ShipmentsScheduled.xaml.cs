using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        public ObservableCollection<Shipment> shipments { get; set; }
        public Shipment selected_shipment { get; set; }
        public Product selected_product { get; set; }
        public ObservableCollection<Product> products_for_shipment { get; set; }

        public ShipmentsScheduled()
        {
            InitializeComponent();
        }

        public void AddNewShipment_Click()
        {

        }

        public void AddComponentToShipment_Click()
        {

        }

        public void DeleteShipment_Click()
        {

        }

        public void DeleteComponentToShipment_Click()
        {

        }
    }
}
