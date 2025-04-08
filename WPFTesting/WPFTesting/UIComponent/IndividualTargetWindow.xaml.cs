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
            ItemModel = new();
            DataContext = ItemModel;
            SetCompletedVisibility(Visibility.Collapsed);
        }

        public void DidCompleteProduction(bool IsTargetCompletable)
        {
            if (IsTargetCompletable)
                SetCompletedVisibility(Visibility.Visible);
            else
                SetCompletedVisibility(Visibility.Collapsed);
        }

        public IndividualTargetWindow(IndividualTargetModel im)
        {
            InitializeComponent();
            _itemmodel = im;
            DataContext = ItemModel;
            SetCompletedVisibility(Visibility.Collapsed);
            //SetGraphDetails();
        }

        public void SetCompletedVisibility(Visibility value)
        {
            CompletedLabel.Visibility = value;
            CompletedLabel.Visibility = value;
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public void GenerateGraphDetails()
        {
            var lg = new LineGraph();
            var dr = 2d;
            lg.Stroke = new SolidColorBrush(Colors.Gold);
            lg.StrokeThickness = 2.5;
            lg.Description = ItemModel.TargetItem.ProductTarget?.ProductName + "s in stock";
            IT_PlotSpace.BottomTitle = "Days";
            IT_PlotSpace.LeftTitle = $"{lg.Description}s";
            if (ItemModel.DaysRun.Count() > 0)
                dr = ItemModel.DaysRun.Max();
            IT_PlotSpace.Title = $"Amount of {lg.Description}s per day for {dr.ToString()} day period";
            IndividualTargetLine.IsAutoFitEnabled = true;
            double[] x = new double[ItemModel.TargetOverDays.Length];
            for ( int i = 0; i < x.Length; i++)
            {
                x[i] = i;
            }

            lg.Plot(x, ItemModel.TargetOverDays);
            IndividualTargetLine.Children.Add(lg);
			var lg2 = new LineGraph();
			lg2.Stroke = new SolidColorBrush(Colors.DarkRed);
			lg2.StrokeThickness = 1.5;
			lg2.Description = ItemModel.TargetItem.ProductTarget?.ProductName + "s Produced";
			lg2.Plot(x, ItemModel.TargetProducedOverDays);
			var polyline = lg2.Children.OfType<Polyline>().FirstOrDefault();
			if (polyline != null)
			{
				polyline.StrokeDashArray = new DoubleCollection { 4, 2 };
			}
			IndividualTargetLine.Children.Add(lg2);
		}

		private void Issue_list_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
