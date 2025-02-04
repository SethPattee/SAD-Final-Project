using FactorSADEfficiencyOptimizer.Models;
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
using WPFTesting.Data;
using WPFTesting.Models;
using WPFTesting.ViewModel;

namespace WPFTesting.UIComponent
{
    /// <summary>
    /// Interaction logic for ProductionAnalysisWindow.xaml
    /// </summary>
    public partial class ProductionAnalysisWindow : Window, INotifyPropertyChanged
    {
        public SupplyChainViewModel scViewModel {  get; set; }
        public ObservableCollection<ProductionTarget> productionTargets { get; set; }
        public ProductionTarget TargetProductionTarget { get; set; }
        private int _daystorun;
        public int DaysToRun { 
            get
            {
                return _daystorun;
            }
            set
            {
                _daystorun = value;
                OnPropertyChanged(nameof(DaysToRun));
            }
        }

        public ProductionAnalysisWindow()
        {
            InitializeComponent();
            this.DataContext = this;
            scViewModel = new SupplyChainViewModel(new InitializedDataProvider());
            productionTargets = new ObservableCollection<ProductionTarget>();
            DaysToRun = 1;
        }

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (AnalysisPeriodTutor is null)
                return;
            if (AnalysisPeriodValue is null)
                return;

            if (DaysToRun >= 8)
            {
                AnalysisPeriodValue.Background = new SolidColorBrush(Colors.AliceBlue);
            }
            else if (DaysToRun < 1)
            {
                DaysToRun = 1;
                AnalysisPeriodValue.Background = new SolidColorBrush(Colors.Transparent);
            }
            else
            {
                AnalysisPeriodValue.Background = new SolidColorBrush(Colors.Transparent);
            }
        }

        private void SetSliderExtraValueVisibility(Visibility new_setting)
        {
            AnalysisPeriodTutor.Visibility = new_setting;
            AnalysisPeriodValue.Visibility = new_setting;
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        private void AnalysisPeriodValue_KeyUp(object sender, KeyEventArgs e)
        {
            if(sender is TextBox textBox)
            {
                if (e.Key == Key.Enter)
                {
                    Int32.TryParse(textBox.Text, out _daystorun);
                    Keyboard.ClearFocus();
                }
                else if (e.Key == Key.Escape)
                {
                    Keyboard.ClearFocus();
                }
            }
        }

        private void AnalysisPeriodValue_LostFocus(object sender, RoutedEventArgs e)
        {
            if(sender is TextBox tb)
            {
                Int32.TryParse(tb.Text, out _daystorun);
            }
        }

        private void AddTargetButton_Click(object sender, RoutedEventArgs e)
        {
            ProductionTarget newtarg = new ProductionTarget()
            {
                DueDate = 3,
                InitAmount = 0,
                IsTargetEnabled = true,
                Status = 0,
                ProductTarget = new Product()
                {
                    ProductName = "New Product",
                    Price = 1,
                    Quantity = 1,
                    Units = ""
                },
                TargetQuantity = 1

            };

            productionTargets.Add(newtarg);
            OnPropertyChanged(nameof(productionTargets));
        }
    }
}
