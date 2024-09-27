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
using YourNamespace;

namespace WPFTesting;

/// <summary>
/// Interaction logic for _.xaml
/// </summary>
public partial class ShippingLine : UserControl
{
    
    public ShippingLine()
    {
        InitializeComponent();
        this.ourShippingLine.X1 = 17;
        this.ourShippingLine.Y1 = 17;
        this.ourShippingLine.X2 = 170;
        this.ourShippingLine.Y2 = 1700;
        this.ourShippingLine.Stroke = new SolidColorBrush(Color.FromRgb(255,0,0));
    }

    public ShippingLine(DraggableBox supplier1, DraggableBox supplier2)
    {
        InitializeComponent();

        this.ourShippingLine.X1 = supplier1.ActualWidth;
        this.ourShippingLine.Y1 = supplier1.ActualHeight;
        this.ourShippingLine.X2 = supplier2.ActualWidth;
        this.ourShippingLine.Y2 = supplier2.ActualHeight;

    }

    private void DeleteShipLine_Click(object sender, RoutedEventArgs e)
    {
        var canvas = this.Parent as Canvas;
        if (canvas == null)
        {
            return;
        }

        canvas.Children.Remove(this);
    }
}
