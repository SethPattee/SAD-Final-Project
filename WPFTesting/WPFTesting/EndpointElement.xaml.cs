using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using WPFTesting.Components;
using WPFTesting.Models;
using WPFTesting.Shapes;

namespace WPFTesting
{
    /// <summary>
    /// Interaction logic for EndpointElement.xaml
    /// </summary>
    public partial class EndpointElement : UserControl, INodeElement
    {
        private Point clickPosition;
        public event EventHandler? RadialClicked;
        public event EventHandler? ElementMoved;

        private SupplierUIValues _nodeUIValues = new EndpointUIValues();

        private bool isDragging = false;
        private decimal Profit = (decimal)1000.00;
        
        public SupplierUIValues nodeUIValues
        {
            get => _nodeUIValues;
            set {
                _nodeUIValues = value;
            }
        }

        public EndpointElement(EndpointUIValues endpointUIValues)
        {
            InitializeComponent();

            this._nodeUIValues = endpointUIValues;
            //add products
            foreach (var x in ((EndpointNode)endpointUIValues.supplier).ComponentInventory)
            {
                this.ComponentsList.Items.Add(x);
            }
            foreach (var x in ((EndpointNode)endpointUIValues.supplier).ProductInventory)
            {
                this.ProductsList.Items.Add(x);
            }

            DataContext = endpointUIValues;
        }

        public void ResizeGrip_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

        }

        public void Click_SenseThisRadial(object sender, RoutedEventArgs e)
        {
            RadialClicked?.Invoke(this, e);
        }

        private void Box_MouseDown(object sender, MouseButtonEventArgs e)
        {
            isDragging = true;
            clickPosition = e.GetPosition(this);
            (sender as UIElement).CaptureMouse();
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
                    _nodeUIValues.xPosition = (int)left;
                    _nodeUIValues.yPosition = (int)top;

                    ElementMoved?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        private void Box_MouseUp(object sender, MouseButtonEventArgs e)
        {
            isDragging = false;
            (sender as UIElement).ReleaseMouseCapture();
        }
    }
}
