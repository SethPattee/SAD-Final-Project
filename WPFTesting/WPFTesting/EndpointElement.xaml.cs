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
using WPFTesting.Shapes;

namespace WPFTesting
{
    /// <summary>
    /// Interaction logic for EndpointElement.xaml
    /// </summary>
    public partial class EndpointElement : UserControl
    {
        public EndpointUIValues EndpointUIValues = new EndpointUIValues()
        {
            Profit = (decimal)1000.00
        };
        public EndpointElement(EndpointUIValues endpointUIValues)
        {
            InitializeComponent();

            this.EndpointUIValues = endpointUIValues;

            DataContext = endpointUIValues;
        }

        public void ResizeGrip_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

        }
    }
}
