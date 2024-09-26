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
    }

    public ShippingLine(DraggableBox supplier1, DraggableBox supplier2)
    {
        InitializeComponent();
       
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
