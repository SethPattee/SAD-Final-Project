﻿using System;
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
using WPFTesting.Shapes;

namespace WPFTesting
{
    /// <summary>
    /// Interaction logic for EndpointElement.xaml
    /// </summary>
    public partial class EndpointElement : UserControl, INodeElement
    {
        public event EventHandler? RadialClicked;

        private SupplierUIValues _nodeUIValues = new EndpointUIValues()
        {
            Profit = (decimal)1000.00
        };
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

            DataContext = endpointUIValues;
        }

        public void ResizeGrip_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

        }

        public void Click_SenseThisRadial(object sender, RoutedEventArgs e)
        {
            RadialClicked?.Invoke(this, e);
        }
    }
}
