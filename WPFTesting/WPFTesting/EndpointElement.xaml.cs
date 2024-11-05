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
using WPFTesting.Models;
using WPFTesting.Shapes;

namespace WPFTesting
{
    /// <summary>
    /// Interaction logic for EndpointElement.xaml
    /// </summary>
    public partial class EndpointElement : UserControl
    {
        private bool isDragging = false;
        private Point clickPosition;
        public EndpointUIValues EndpointUIValues = new EndpointUIValues()
        {
            Profit = (decimal)1000.00
        };
        public EndpointElement(EndpointUIValues endpointUIValues)
        {
            InitializeComponent();

            this.EndpointUIValues = endpointUIValues;
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
                    EndpointUIValues.xPosition = (int)left;
                    EndpointUIValues.yPosition = (int)top;

                    //BoxChanged?.Invoke(this, EventArgs.Empty); //pulled from supplierElement, do we need this event watched?
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
