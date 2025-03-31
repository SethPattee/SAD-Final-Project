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

        public void AddProductToEndpoint_Click(object sender, RoutedEventArgs e)
        {
            EndpointVM.ProductInventory.Add(new Product());
        }

        public void AddComponentToEndpoint_Click(object sender, RoutedEventArgs e)
        {
            EndpointVM.ComponentInventory.Add(new Product());
        }

        public void AddProductLineToEndpoint_Click(object sender, RoutedEventArgs e)
        {
            EndpointVM.ProductionList.Add(new ProductLine()
            {
                ResultingProduct = new Product(),
                Components = new ObservableCollection<Product>()
                {
                    new Product()
                }
            });
        }

        public void AddComponentToProductLineInEndpoint_Click(object sender, RoutedEventArgs e)
        {
            if(sender is Button b)
            {
                ((ProductLine)b.DataContext).Components.Add(new Product());
            }
        }

        public void AddActiveDeliveryLineToEndpoint_Click(object sender, RoutedEventArgs e)
        {
            EndpointVM.ActiveDeliveryLines.Add(new DeliveryLine()
            {
                DeliveryItem = new Product()
            });
        }

        public void AddPastDeliveryLineToEndpoint_Click(object sender, RoutedEventArgs e)
        {
            EndpointVM.PastDeliveryLines.Add(new DeliveryLine()
            {
                DeliveryItem = new Product()
            });
        }


        public void MoveDeliveryLineToActive_Click(object sender, RoutedEventArgs e)
        {
            if(sender is Button b)
            {
                EndpointVM.ActiveDeliveryLines.Add((DeliveryLine)b.DataContext);
                EndpointVM.PastDeliveryLines.Remove((DeliveryLine)b.DataContext);
            }
        }

        public void MoveDeliveryLineToPast_Click(object sender, RoutedEventArgs e)
        {
            if(sender is Button b)
            {
                EndpointVM.PastDeliveryLines.Add((DeliveryLine)b.DataContext);
                EndpointVM.ActiveDeliveryLines.Remove((DeliveryLine)b.DataContext);
            }
        }

        public void DeleteProductFromEndpointInventory_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button b)
            {
                EndpointVM.ProductInventory.Remove((Product)b.DataContext);
            }
        }

        public void DeleteComponentFromEndpointInventory_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button b)
            {
                EndpointVM.ComponentInventory.Remove((Product)b.DataContext);
            }
        }

        public void DeleteComponentFromEndpointProductLine_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button b)
            {
                var plContextTarget = GetAncestorOfType(VisualTreeHelper.GetParent(b));

                ((ProductLine)plContextTarget.DataContext).Components.Remove((Product)b.DataContext);
            }
        }


        private ListView GetAncestorOfType(DependencyObject element)
        {
            var parent = VisualTreeHelper.GetParent(element);
            if (parent is null)
                return null;

            if (parent.GetType() == typeof(ListView))
                return (ListView)parent;
            else
            {
                return (ListView)GetAncestorOfType((DependencyObject)parent);
            }
        }

        public void DeleteProductLineFromEndpoint_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button b)
            {
                EndpointVM.ProductionList.Remove((ProductLine)b.DataContext);
            }
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
