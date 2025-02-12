using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using FactorySADEfficiencyOptimizer.Components;
using FactorySADEfficiencyOptimizer.Models;
using FactorySADEfficiencyOptimizer.Shapes;
using FactorySADEfficiencyOptimizer.ViewModel;
using FactorySADEfficiencyOptimizer.ViewShared;

namespace YourNamespace
{
    public partial class SupplierElement : UserControl, INodeElement
    {
        private bool isDragging = false;
        private Point clickPosition;
        private SupplierUIValues _nodeUIValues = new SupplierUIValues()
        {
            supplier = new Supplier()
        }; // Temporary replacement until we get MVVM data binding in place
        public SupplierUIValues NodeUIValues
        {
            get => _nodeUIValues;
            set
            {
                _nodeUIValues = value;
            }
        }
        public event EventHandler? BoxChanged;
        public event EventHandler? RadialClicked;
        public event EventHandler? BoxDeleted;
        public event PropertyChangedEventHandler PropertyChanged;
        public List<Product> Products {  get; set; } = new List<Product>();
        public double X { get; set; }
        public double Y { get; set; }

        public string CornerClicked = "Center";


        public SupplierElement(SupplierUIValues supplierValues)
        {
            InitializeComponent();
            this._nodeUIValues = supplierValues;
            //this.BoxTitle.Text = supplierValues.supplier.Name;
            X = supplierValues.Position.X;
            Y = supplierValues.Position.Y;
            this._nodeUIValues.supplier = supplierValues.supplier;
            if (this._nodeUIValues.supplier.Name == "" || this._nodeUIValues.supplier.Name == null)
                this._nodeUIValues.supplier.Name = "New Supplier";
            FillProductDisplay();
            DataContext = supplierValues;
        }
        public void EmptyProductDisplay()
        {
            this.ItemsList.Items.Clear();
        }
        public void FillProductDisplay()
        {
            this.ItemsList.Items.Clear();
            foreach (var x in _nodeUIValues.supplier.ProductInventory)
            {
                this.ItemsList.Items.Add(x);
            }
        }

        private void Box_MouseDown(object sender, MouseButtonEventArgs e)
        {
            isDragging = true;
            clickPosition = e.GetPosition(this);
            (sender as UIElement).CaptureMouse();
        }

        private void Click_SenseThisRadial(object sender, RoutedEventArgs e)
        {
            Button b = (Button)sender;
            this.CornerClicked = b.Name;
            RadialClicked?.Invoke(this, EventArgs.Empty);
            this.CornerClicked = "Center";
        }

        private void Box_MouseMove(object sender, MouseEventArgs e) // Marked for Change
        {
            if (isDragging)
            {
                var canvas = this.Parent as Canvas;
                if (canvas != null)
                {
                    var mousePos = e.GetPosition(canvas);
                    double left = mousePos.X - clickPosition.X;
                    double top = mousePos.Y - clickPosition.Y;

                    Canvas.SetLeft(this, left);
                    Canvas.SetTop(this, top);
                    _nodeUIValues.Position = new System.Drawing.Point((int)left, (int)top);

                    BoxChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        private void Box_MouseUp(object sender, MouseButtonEventArgs e)
        {
            isDragging = false;
            (sender as UIElement).ReleaseMouseCapture();
        }

        private void ResizeThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            double newWidth = this.Width + e.HorizontalChange;
            double newHeight = this.Height + e.VerticalChange;

            if (newWidth >= 50) this.Width = newWidth;
            if (newHeight >= 30) this.Height = newHeight;

            BoxChanged?.Invoke(this, EventArgs.Empty);
        }

        private void DeleteBox_Click(object sender, RoutedEventArgs e)
        {
            var canvas = this.Parent as Canvas;
            if (canvas == null)
            {
                return; 
            }

            canvas.Children.Remove(this);
            BoxDeleted?.Invoke(this, EventArgs.Empty);
            if (Window.GetWindow(this) is MainWindow mainWindow)
            {
                mainWindow.UpdateBoxTracker();
            }
        }

        private void ChangeColor_Click(object sender, RoutedEventArgs e)
        {
            var random = new Random();
            byte r = (byte)random.Next(256);
            byte g = (byte)random.Next(256);
            byte b = (byte)random.Next(256);

            boxBorder.Background = new SolidColorBrush(Color.FromRgb(r, g, b));
            random = new Random();
            r = (byte)random.Next(256);
            g = (byte)random.Next(256);
            b = (byte)random.Next(256);

            boxBorder.BorderBrush = new SolidColorBrush(Color.FromRgb(r, g, b));
            BoxChanged?.Invoke(this, EventArgs.Empty);

        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);
        }
    }

    public class BoxConnection : BoxConnectionBase
    {
    }

    public class BoxProperties : BoxPropertiesBase
    {
    }
}
