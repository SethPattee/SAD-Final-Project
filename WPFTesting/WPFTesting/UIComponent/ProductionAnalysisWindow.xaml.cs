using FactorSADEfficiencyOptimizer.Methods;
using FactorSADEfficiencyOptimizer.Models;
using FactorSADEfficiencyOptimizer.UIComponent;
using FactorSADEfficiencyOptimizer.ViewModel;
using FactorySADEfficiencyOptimizer.Models;
using FactorySADEfficiencyOptimizer.Models.AnalyzerTrackers;
using FactorySADEfficiencyOptimizer.UIComponent.EventArguments;
using FactorySADEfficiencyOptimizer.Shapes;
using FactorySADEfficiencyOptimizer.ViewModel;
using InteractiveDataDisplay.WPF;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

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
                    Quantity = 1
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
            double[] GraphDays = new double[(int)simModel.DaysToRun + 1];
            for (int i = 0; i <= simModel.DaysToRun; i++)
            {
                GraphDays[i] = i;
            }
            double[] BalancePerDay = simModel.GetBalancePerDayForGraph();
            var lg = new LineGraph();
            lg.Stroke = GetIteratedLineColor();
            lg.Description = "Balance per Day";
            lg.StrokeThickness = 3;
            plotter.BottomTitle = "Day";
            plotter.LeftTitle = "Balance (in 1000 $s)";
            plotter.Title = "Balance-Per-Day during production.";
            plotter.PlotOriginX = 1;
            plotter.PlotOriginY = BalancePerDay.Min();
            linegraph.IsAutoFitEnabled = true;
            lg.Plot(GraphDays, BalancePerDay);
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
                        if (shipment.Sender.Name == supp.Supplier.Name)
                            shipment.Sender = supp.Supplier;
                        else if(shipment.Receiver.Name == supp.Supplier.Name)
                            shipment.Receiver = supp.Supplier;
                    }

                    foreach(var supp in simModel.EndpointList)
                    {
                        if(shipment.Receiver.Name == supp.Supplier.Name)
                            shipment.Receiver = supp.Supplier;
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

        private void ProdTargetList_Click(object sender, RoutedEventArgs e)
        {
            if(sender is ListView s)
            {
                if (s.SelectedItem is null || ((ProductionTarget)s.SelectedItem).ProductTarget is null)
                    return;

                var product_target = (ProductionTarget)s.SelectedItem!;
                var product_name = product_target.ProductTarget!.ProductName;
                IndividualTargetWindow individualTarget = MakeNewIndividualTargetWindow(product_target, product_name);

                individualTarget.ItemModel.Issues = GetAllIssues(product_name);
                individualTarget.Owner = this;
                individualTarget.Show();
            }
            else if (sender is MenuItem m)
            {
                if(m.DataContext is null || ((ProductionTarget)m.DataContext).ProductTarget is null)
                        return;

                var product_target = (ProductionTarget)m.DataContext;
                var product_name = product_target.ProductTarget!.ProductName;
                IndividualTargetWindow individualTarget = MakeNewIndividualTargetWindow(product_target, product_name);

                individualTarget.ItemModel.Issues = GetAllIssues(product_name);
                individualTarget.Owner = this;
                individualTarget.Show();
            }
            else if (sender is Button b)
            {
                if (b.DataContext is null || ((ProductionTarget)b.DataContext).ProductTarget is null)
                    return;

                var product_target = (ProductionTarget)b.DataContext;
                var product_name = product_target.ProductTarget!.ProductName;
                IndividualTargetWindow individualTarget = MakeNewIndividualTargetWindow(product_target, product_name);

                individualTarget.ItemModel.Issues = GetAllIssues(product_name);
                individualTarget.Owner = this;
                individualTarget.Show();
            }
        }
        private void Icon_Click(object sender, RoutedEventArgs e)
        {
            //if (sender is Button b)
            //{
            //    System.Windows.Controls.StackPanel a = (System.Windows.Controls.StackPanel)b.Parent;
            //    var c = a.Parent;
            //    Grid_MouseRightButtonDown(c, null);
            //}
            ProdTargetList_Click(sender, e);
            //OpenBillOfMaterials_Click(sender, e);
        }
        
        private ObservableCollection<string> GetAllIssues(string product_name)
        {
            var ocs = new ObservableCollection<string>();
            if (simModel.IssueLog.Count == 0)
            {
                ocs.Add("No issues.");
                return ocs;
            }

            // For each '!': Kaboom!
            // Gotta find a better solution, maybe better design, but eh.
            foreach (var issue in simModel.IssueLog.Where(x => x.ProductionTarget.ProductTarget!.ProductName == product_name))
            {
                string shipmentaddition = "";
                if (issue.Severity == StatusEnum.Failure)
                    shipmentaddition = "Critical failure occurred! Could not complete target.";
                if (issue.Solution.Action == ActionEnum.addedShipment)
                    shipmentaddition = $"Added a shipment: {issue.Solution.neededProduct.Quantity}" +
                        $" {issue.Solution.neededProduct.ProductName}" +
                        $" ordered for ${issue.Solution.neededProduct.Price}.";

                ocs.Add($"Item {product_name} issue on {issue.DayFound}: {issue.Severity.ToString()}" +
                    $"{shipmentaddition}");
            }

            return ocs;
        }

        private IndividualTargetWindow MakeNewIndividualTargetWindow(ProductionTarget pt, string product_name)
        {
            //var idwt = simModel.ExtractProductionTargetChanges(product_name)!;
            //double[] targetQuantityOverDays = new double[idwt.Count];
            double[] targetQuantityOverDays = simModel.ExtractProductionTargetChanges(product_name).Select<ProductionTarget?, double>(x => (double)x!.CurrentAmount).ToArray();

            var newVM = new IndividualTargetModel()
            {
                TargetItem = pt,
                DaysRun = simModel.GetQuantityPerDayForGraph(product_name),
                TargetOverDays = targetQuantityOverDays,
                Issues = new ObservableCollection<string>(),
                DayCompleted = simModel.GetDayCompletedFor(product_name)
            };
            var newITW = new IndividualTargetWindow();
            newITW.ItemModel = newVM;
            newITW.DataContext = newITW.ItemModel;
            newITW.GenerateGraphDetails();
            return newITW;

        }

        private ProductionTarget _selectedTarget;

        private void Grid_MouseRightButtonDown(object sender, MouseButtonEventArgs? e)
        {
            if(sender is Grid g)
            {
                _selectedTarget = ((ProductionTarget)g.DataContext);
            }
        }

        private void OpenBillOfMaterials_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedTarget is null)
            {
                BillofMaterials bil = new BillofMaterials();
                bil.TotalExpenses = simModel.Snapshots.Sum(x => x.todaysSpending);
                bil.TargetName = "All Targets";
                Dictionary<string, Product> components_used = ExtractUsedComponents();
                bil.List = new ObservableCollection<Product>(components_used.Values);

                bil.Owner = this;
                bil.Show();
                return;
            }

            if (simModel.ProductionTargets.Any(x => x.Status == StatusEnum.NotDone))
                return;

            ObservableCollection<Product> spentProducts = new();
            BillofMaterials bom = new BillofMaterials();
            bom.TotalExpenses = simModel.Snapshots.Sum(x => x.todaysSpending);
            bom.TargetName = _selectedTarget.ProductTarget!.ProductName;
            Dictionary<string, Product> component_use = ExtractUsedComponents();
            bom.List = new ObservableCollection<Product>(component_use.Values);

            bom.Owner = this;
            bom.Show();
        }

        private Dictionary<string, Product> ExtractUsedComponents()
        {
            Dictionary<string, Product> component_use = new();
            foreach (var item in simModel.Snapshots)
            {
                foreach (var component in item.ComponentsUsed)
                {
                    if (!component_use.ContainsKey(component.ProductName))
                        component_use.Add(component.ProductName, component);
                    else
                        component_use[component.ProductName].Quantity += component.Quantity;
                }
            }

            return component_use;
        }

        //private void PersistChangesToGraphButton_Click(object sender, RoutedEventArgs e)
        //{
        //    ShipmentListMouseButtonEventArgs s = new();

        //    s.AssignList(simModel.ShipmentList);

        //    SaveChangesToShipmentsEvent?.Invoke(this, s);
        //}

        //public EventHandler? SaveChangesToShipmentsEvent;


		private void DeleteProductionTargetButton_Click(object sender, RoutedEventArgs e)
		{
			if (sender is Button b)
			{
                ProductionTarget targ = simModel.ProductionTargets.First(s => s == (ProductionTarget)b.DataContext);
                simModel.ProductionTargets.Remove(targ);
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
            return Binding.DoNothing;
        }
    }
    public class StatusToImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is StatusEnum status)
            {
                return status switch
                {
                    StatusEnum.Warning => new Uri(@"./Resources/MinorFailIcon.png", UriKind.Relative),
                    StatusEnum.Failure => new Uri(@"./Resources/CriticalFailIcon.png", UriKind.Relative),
                    StatusEnum.Success => new Uri(@"./Resources/SuccessIcon.png", UriKind.Relative),
                    StatusEnum.NotDone => new Uri(@"./Resources/UnrunTargetIcon.png", UriKind.Relative),
                    _ => new Uri(@"./Resources/UnrunTargetIcon.png", UriKind.Relative)
                };
            }
            return new Uri(@"./Resources/UnrunTargetIcon.png", UriKind.Relative);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
}
