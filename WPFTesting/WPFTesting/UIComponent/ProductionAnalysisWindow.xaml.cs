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
using WPFTesting.ViewModel;

namespace WPFTesting.UIComponent
{
    /// <summary>
    /// Interaction logic for ProductionAnalysisWindow.xaml
    /// </summary>
    public partial class ProductionAnalysisWindow : Window
    {
        public SupplyChainViewModel scViewModel {  get; set; }
        public int DaysToRun { 
            get; 
            set; 
        }

        public ProductionAnalysisWindow()
        {
            InitializeComponent();

            DaysToRun = 1;
        }

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (AnalysisPeriodTutor is null)
                return;
            if (AnalysisPeriodValue is null)
                return;

            if(DaysToRun >= 8)
            {
                SetSliderExtraValueVisibility(Visibility.Visible);
            }
            else if(DaysToRun < 1)
            {
                DaysToRun = 1;
                SetSliderExtraValueVisibility(Visibility.Hidden);
            }
            else
            {
                SetSliderExtraValueVisibility(Visibility.Hidden);
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
    }
}
