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
using YourNamespace;

namespace WPFTesting;

/// <summary>
/// Interaction logic for _.xaml
/// </summary>
public partial class ShipingLine : UserControl
{

    public string CardinalJoint = "Center";
    public string Label { get; set; }
    public List<ShippingDetails> ShippingDetails { get; set; }
    public Guid Id { get; private set; }
    public Shipment ShipmentOrder { get; set; }
    public INodeElement Source { get; set; }
    public INodeElement Destination { get; set; }
    public string FromJoiningBoxCorner { get; set; }
    public string ToJoiningBoxCorner { get; set; }



    public ShipingLine(Guid? id = null)
    {
        InitializeComponent();
        this.ShipmentOrder = new Shipment();
        DataContext = this;
        ShippingDetails = new List<ShippingDetails>();
        Id = id ?? Guid.NewGuid();
        this.Label = "This is a lable";
        this.ourShippingLine.X1 = 17;
        this.ourShippingLine.Y1 = 17;
        this.ourShippingLine.X2 = 170;
        this.ourShippingLine.Y2 = 170;
    }
    

    public void AddShippingDetail(ShippingDetails detail)
    {
        ShippingDetails.Add(detail);
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
public class ShippingDetails
{
    public string? Item { get; set; }
    public double? Weight { get; set; }
    public int? Quantity { get; set; }
    public string? Type { get; set; }

    public ShippingDetails(string item, double weight, int quantity, string type)
    {
        Item = item;
        Weight = weight;
        Quantity = quantity;
        Type = type;
    }
}


