using FactorySADEfficiencyOptimizer.Models;
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

namespace FactorySADEfficiencyOptimizer.UIComponent
{
    /// <summary>
    /// Interaction logic for EndpointDetailsWindow.xaml
    /// </summary>
    public partial class EndpointDetailsWindow : Window, INotifyPropertyChanged
    {
        public EndpointDetailsWindow()
        {
            InitializeComponent();
            EndpointVM = new();
            DataContext = EndpointVM;
        }

        public EndpointDetailsWindow(EndpointNode vm)
        {
            InitializeComponent();
            EndpointVM = vm;
            DataContext = EndpointVM;
        }

        public void DeletePastDeliveryButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button b)
            {
                EndpointVM.PastDeliveryLines.Remove((DeliveryLine)b.DataContext);
            }
        }

        public void DeleteActiveDeliveryButton_Click(object sender, RoutedEventArgs? e)
        {
            if (sender is Button b)
            {
                EndpointVM.ActiveDeliveryLines.Remove((DeliveryLine)b.DataContext);
            }
        }

        private EndpointNode _edm;
        public EndpointNode EndpointVM
        {
            get => _edm;
            set
            {
                _edm = value;
                OnPropertyChanged(nameof(EndpointVM));
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    
        //public class EndpointDetailsModel : INotifyPropertyChanged
        //{
        //    public EndpointDetailsModel()
        //    {
        //        _name = "";
        //        _products = new();
        //        _components = new();
        //        _productionLines = new();
        //        _pastDLs = new();
        //        _activeDLs = new();
        //    }

        //    private string _name;
        //    public string EndpointName
        //    {
        //        get => _name;
        //        set
        //        {
        //            _name = value;
        //            OnPropertyChanged(nameof(EndpointName));
        //        }
        //    }

        //    private ObservableCollection<Product> _products;
        //    public ObservableCollection<Product> ProductInventory
        //    {
        //        get => _products;
        //        set
        //        {
        //            _products = value;
        //            OnPropertyChanged(nameof(ProductInventory));
        //        }
        //    }

        //    private ObservableCollection<Product> _components;
        //    public ObservableCollection<Product> ComponentInventory
        //    {
        //        get => _components;
        //        set
        //        {
        //            _components = value;
        //            OnPropertyChanged(nameof(ComponentInventory));
        //        }
        //    }

        //    private ObservableCollection<ProductLine> _productionLines;
        //    public ObservableCollection<ProductLine> ProductionLines
        //    {
        //        get => _productionLines;
        //        set
        //        {
        //            _productionLines = value;
        //            OnPropertyChanged(nameof(ProductionLines));
        //        }
        //    }

        //    private ObservableCollection<DeliveryLine> _activeDLs;
        //    public ObservableCollection<DeliveryLine> ActiveDeliveryLines
        //    {
        //        get => _activeDLs;
        //        set
        //        {
        //            _activeDLs = value;
        //            OnPropertyChanged(nameof(ActiveDeliveryLines));
        //        }
        //    }

        //    private ObservableCollection<DeliveryLine> _pastDLs;
        //    public ObservableCollection<DeliveryLine> PastDeliveryLines
        //    {
        //        get => _pastDLs;
        //        set
        //        {
        //            _activeDLs = value;
        //            OnPropertyChanged(nameof(PastDeliveryLines));
        //        }
        //    }

        //    public event PropertyChangedEventHandler PropertyChanged;
        //    protected virtual void OnPropertyChanged(string propertyName)
        //    {
        //        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        //    }
        //}
    }
}
