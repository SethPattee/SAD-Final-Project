using FactorSADEfficiencyOptimizer.Models;
using FactorySADEfficiencyOptimizer.ViewModel;
using InteractiveDataDisplay.WPF;
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
        private IndividualTargetModel _itemmodel;
        public IndividualTargetModel ItemModel {
            get => _itemmodel;
            set
            {
                _itemmodel = value;
                OnPropertyChanged(nameof(ItemModel));
            }
        }


        public IndividualTargetWindow()
        {
            InitializeComponent();
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        //public void SetGraphDetails()
        //{
        //    double[] GraphOfSupplyIncreasePerDay = ItemModel.TargetOverDays.Select(x => (double)x.CurrentAmount).ToArray();
        //    var lg = new LineGraph();
        //    lg.Stroke = new SolidColorBrush(Colors.Gold);
        //    lg.StrokeThickness = 2;
        //    lg.Description = ItemModel.TargetItem.ProductTarget?.ProductName;
        //    IT_PlotSpace.BottomTitle = "Days";
        //    IT_PlotSpace.LeftTitle = $"{lg.Description}s";
        //    var dr = ItemModel.DaysRun.Max();
        //    IT_PlotSpace.Title = $"Amount of {lg.Description}s per day for {dr.ToString()} period";
        //    IndividualTargetLine.IsAutoFitEnabled = true;
        //    lg.Plot(ItemModel.DaysRun, GraphOfSupplyIncreasePerDay);
        //    IndividualTargetLine.Children.Add(lg);
        //}

        private void Issue_list_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
