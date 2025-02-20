using FactorSADEfficiencyOptimizer.Methods;
using FactorSADEfficiencyOptimizer.Models;
using FactorSADEfficiencyOptimizer.UIComponent;
using FactorSADEfficiencyOptimizer.ViewModel;
using InteractiveDataDisplay.WPF;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net.WebSockets;
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
using FactorySADEfficiencyOptimizer.Data;
using FactorySADEfficiencyOptimizer.Models;
using FactorySADEfficiencyOptimizer.ViewModel;
using System.Globalization;

namespace FactorySADEfficiencyOptimizer.UIComponent
{
    /// <summary>
    /// Interaction logic for ProductionAnalysisWindow.xaml
    /// </summary>
    public partial class ProductionAnalysisWindow : Window, INotifyPropertyChanged
    {
        public event EventHandler? ShipmentHighlightPassoverHandler;
        public double ProdTargetListWidth { get; set; }
        private AnalizorModel _simModel;
        public AnalizorModel simModel { 
            get => _simModel;
            set
            {
                _simModel = value;
                OnPropertyChanged(nameof(simModel));
            } 
        }
        public ProductionTarget TargetProductionTarget { get; set; }


        public ProductionAnalysisWindow(SupplyChainViewModel model)
        {
            InitializeComponent();
            simModel = new AnalizorModel(model);
            this.DataContext = this;
            simModel.DaysToRun = 1;
            double[] x = new double[200];
            for (int i = 0; i < x.Length; i++)
                x[i] = 3.1415 * i / (x.Length - 1);

            for (int i = 0; i < 25; i++)
            {
                var lg = new LineGraph();
                lg.Stroke = new SolidColorBrush(Color.FromArgb(255, 0, (byte)(i * 10), 0));
                lg.Description = String.Format("Data series {0}", i + 1);
                lg.StrokeThickness = 2;
                linegraph.Children.Add(lg);
                lg.Plot(x, x.Select(v => Math.Sin(v + i / 10.0)).ToArray());
            }
            //UpdatePlot();
        }

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (AnalysisPeriodTutor is null)
                return;
            if (AnalysisPeriodValue is null)
                return;

            if (simModel.DaysToRun >= 8)
            {
                AnalysisPeriodValue.Background = new SolidColorBrush(Colors.AliceBlue);
            }
            else if (simModel.DaysToRun < 1)
            {
                simModel.DaysToRun = 1;
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
                    int i = 0;
                    Int32.TryParse(textBox.Text, out i);
                    simModel.DaysToRun = i;
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
                int i = 0;
                Int32.TryParse(tb.Text, out i);
                simModel.DaysToRun = i;
            }
        }

        private void AddTargetButton_Click(object sender, RoutedEventArgs e)
        {
            ProductionTarget newtarg = GetNewProductionTarget();
            simModel.ProductionTargets.Add(newtarg);
            OnPropertyChanged(nameof(simModel));
        }

        private static ProductionTarget GetNewProductionTarget()
        {
            return new ProductionTarget()
            {
                DueDate = 3,
                CurrentAmount = 0,
                IsTargetEnabled = true,
                Status = 0,
                ProductTarget = new Product()
                {
                    ProductName = "Target item",
                    Price = 1,
                    Quantity = 1,
                    Units = ""
                },
                TargetQuantity = 1
            };
        }

        private void StartSim_Click(object? sender,  RoutedEventArgs? e)
        {
            simModel.PassTimeUntilDuration(simModel.DaysToRun);
            linegraph.Children.Clear();
            RenderLineResults();
        }

        private void LineSelectedFromWindow_MouseDown(object? sender, EventArgs? e)
        {
            ShipmentHighlightPassoverHandler?.Invoke(this, e);
        }

        private void RenderLineResults()
        {
            double[] GraphDays = new double[(int)simModel.DaysToRun];
            for (int i = 0; i < simModel.DaysToRun; i++)
            {
                GraphDays[i] = i;
            }
            double[] BalancePerDay = simModel.GetBalancePerDayForGraph();
            var lg = new LineGraph();
            lg.Stroke = GetIteratedLineColor();
            lg.Description = "Financial State";
            lg.StrokeThickness = 1;
            plotter.BottomTitle = "Day";
            plotter.LeftTitle = "Balance (in dollars)";
            plotter.Title = "Balance-Per-Day during production.";
            plotter.PlotOriginX = 1;
            plotter.PlotOriginY = BalancePerDay.Min();
            linegraph.IsAutoFitEnabled = true;
            linegraph.Children.Add(lg);
        }

        private SolidColorBrush GetIteratedLineColor()
        {
            Random random = new Random();
            return new SolidColorBrush(Color.FromArgb(255, (byte)random.Next(0,255), (byte)random.Next(0, 255), (byte)random.Next(0, 255)));
        }

        private void OpenShipmentWindow_Click(object? sender, RoutedEventArgs? e)
        {
            if(sender is Button button)
            {
                var ShipmentWindow = new ShipmentsScheduled(simModel);
                ShipmentWindow.Owner = this;
                ShipmentWindow.SaveShipmentDetails += ReworkShipmentSimulation;
                ShipmentWindow.SelectShipmentLineHandler += LineSelectedFromWindow_MouseDown;
                ShipmentWindow.Show();
            }
        }

        private void ReworkShipmentSimulation(object? sender, EventArgs? s)
        {
            if(s is SavedShipmentEventArgs rea)
            {
                List<Shipment> convertedShipments = new List<Shipment>();
                foreach (var shipment in rea.SavedShipmentList)
                {
                    foreach(var supp in simModel.SupplierList)
                    {
                        if (shipment.Sender.Name == supp.supplier.Name)
                            shipment.Sender = supp.supplier;
                        else if(shipment.Receiver.Name == supp.supplier.Name)
                            shipment.Receiver = supp.supplier;
                    }

                    foreach(var supp in simModel.EndpointList)
                    {
                        if(shipment.Receiver.Name == supp.supplier.Name)
                            shipment.Receiver = supp.supplier;
                    }
                    convertedShipments.Add(shipment);
                }
                simModel.ShipmentList = new ObservableCollection<Shipment>(convertedShipments);
            }
        }

        private void ClrPcker_Background_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            foreach (var item in linegraph.Children.OfType<LineGraph>())
            {
                if(e is not null)
                    item.Stroke = new SolidColorBrush((Color)e.NewValue!);
            }
        }
        //private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        //{
        //    ResizeAnyProductionTargetRows();
        //}

        //private void ResizeAnyProductionTargetRows()
        //{
        //    ProdTargetListWidth = ProdTargetList.ActualWidth - 16;
        //    OnPropertyChanged(nameof(ProdTargetListWidth));
        //}

        //private void UpdatePlot()
        //{
        //    if (linegraph == null) return;

        //    int xData = TargetProductionTarget.DueDate;
        //    int yData = (int)Math.Round(TargetProductionTarget.InitAmount);

        //    linegraph.Plot(xData, yData);
        //    linegraph.Description = "Production Target Over Time";
        //}


    }
    public class StatusToTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is StatusEnum status)
            {
                return status switch
                {
                    StatusEnum.Warning => "WARNING",
                    StatusEnum.Failure => "FAILURE",
                    StatusEnum.Success => "SUCCESS",
                    StatusEnum.NotDone => "Not Run Yet",
                    _ => "Unknown"
                };
            }
            return "Unknown";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Convert back logic (if needed)
            return Binding.DoNothing;
        }
    }
}
