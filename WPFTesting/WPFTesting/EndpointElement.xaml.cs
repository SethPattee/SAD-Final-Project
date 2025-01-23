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
using WPFTesting.Data;
using WPFTesting.Models;
using WPFTesting.Shapes;
using WPFTesting.ViewModel;

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
        public event EventHandler? ElementClicked;
        public Guid Id { get; set; }

        private SupplierUIValues _nodeUIValues = new EndpointUIValues();

        private bool isDragging = false;

        public SupplierUIValues NodeUIValues
        {
            get => _nodeUIValues;
            set
            {
                _nodeUIValues = value;
                
            }
        }

        public void PopulateElementLists()
        {
            EndpointTitle.Text = NodeUIValues.supplier.Name;
            this.ComponentsList.Items.Clear();
            foreach (var x in ((EndpointNode)_nodeUIValues.supplier).ComponentInventory)
            {
                this.ComponentsList.Items.Add(x);
            }

            this.ProductsList.Items.Clear();
            foreach (var x in ((EndpointNode)_nodeUIValues.supplier).ProductInventory)
            {
                this.ProductsList.Items.Add(x);
            }
        }

        public EndpointElement(EndpointUIValues endpointUIValues)
        {
            InitializeComponent();
            this._nodeUIValues = endpointUIValues;
            //add products
            PopulateElementLists();
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

        public void Click_SelectElement(object sender, MouseButtonEventArgs e)
        {
            ElementClicked?.Invoke(this, e);
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
