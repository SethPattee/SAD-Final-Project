using FactorSADEfficiencyOptimizer.Models;
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

namespace FactorySADEfficiencyOptimizer.UIComponent
{
    /// <summary>
    /// Interaction logic for IndividualTargetWindow.xaml
    /// </summary>
    public partial class IndividualTargetWindow : Window, INotifyPropertyChanged
    {

        public IndividualTargetModel ItemModel { get; set; }


        public IndividualTargetWindow()
        {
            InitializeComponent();
            ItemModel = new();
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }


        private void Issue_list_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
