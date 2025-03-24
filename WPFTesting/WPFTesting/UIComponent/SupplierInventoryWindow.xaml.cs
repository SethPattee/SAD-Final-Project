using FactorySADEfficiencyOptimizer.Models;
using FactorySADEfficiencyOptimizer.UIComponent.EventArguments;
using System;
using System.Collections.Generic;
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

namespace FactorySADEfficiencyOptimizer.UIComponent
{
    /// <summary>
    /// Interaction logic for SupplierInventoryWindow.xaml
    /// </summary>
    public partial class SupplierInventoryWindow : Window, INotifyPropertyChanged
    {
        public SupplierInventoryWindow WithViewModel(Supplier vm)
        {
            SupplierVM = vm;
            DataContext = SupplierVM;
            return this;
        }

        public EventHandler? SaveSupplierHandler;
        public void SaveSupplierDetails_Click(object sender, RoutedEventArgs args)
        {
            SaveSupplierHandler?.Invoke(this, new SaveSupplierEventArgs() { supplier = _supplierVM });
            Close();
        }

        public void AddItemToInventory_Click(object sender, RoutedEventArgs args)
        {
            SupplierVM.ProductInventory.Add(
                    new Product()
                    {
                        ProductName = "New item",
                        Quantity = 1,
                        Price = 0
                    }
                );
        }

        public void DeleteItemButton_Click(object sender, RoutedEventArgs args)
        {
            if(sender is Button b)
            {
                SupplierVM.ProductInventory.Remove(((Product)b.DataContext));
            }
        }

        private Supplier _supplierVM;

        public Supplier SupplierVM
        {
            get
            {
                return _supplierVM;
            }
            set
            {
                _supplierVM = value;
                OnPropertyChanged(nameof(SupplierVM));
            }
        }


        public SupplierInventoryWindow()
        {
            InitializeComponent();
        }


        public event PropertyChangedEventHandler? PropertyChanged;
        public void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
