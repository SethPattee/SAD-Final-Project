using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using WPFTesting.Data;
using WPFTesting.Shapes;
using WPFTesting.ViewModel;
using WPFTesting.Models;
using WPFTesting;
using System.Windows.Documents;
using System.Windows.Data;
using System.Reflection;
using WPFTesting.Components;
using System.Net;
using System.Xml.Linq;

namespace YourNamespace
{
    public partial class MainWindow : Window
    {
        private ObservableCollection<string> boxList = new ObservableCollection<string>();
        public SupplyChainViewModel ViewModel { get; set; }
        private SupplierElement selectedBoxForRemoval = null;
        private SupplierElement firstSelectedBox = null;
        private Popup? connectionSelectionPopup;
        private bool isRemovingConnection = false;
        private bool isAddingConnection = false;
        private bool MouseIsCaptured = false;
        private bool IsDestinationSearching = false;
        private ShippingLine? targetShipingLine = null;
        private Shipment? targetShipment = null;
        private List<ShippingLine> ShipmentList = new List<ShippingLine>();
        private Product selectedProduct;
        public ObservableCollection<Product> EndpointProductListData { get; set; }
        public ObservableCollection<Product> EndpointComponentListData { get; set; }
        public ObservableCollection<Product> EndpointDeliveryListData { get; set; }
        public (SupplierElement?, EndpointElement?, Shipment?) selectedElement = new();
        public ObservableCollection<ComponentToProductTransformer> EndpointProductionLineData { get; set; }
        public event EventHandler? BoxChanged;
        public event EventHandler? LineChanged;


    public MainWindow()
        {
            InitializeComponent();
            WindowState = WindowState.Maximized;
            ViewModel = new SupplyChainViewModel(new InitializedDataProvider());
            MakeEndpoint();
            DataContext = ViewModel;
            Initialize();
            sideBar.BoxList.ItemsSource = boxList;
        }

        private void Initialize()
        {
            ViewModel.Load();
            AddDraggableBoxes();
        }

        private void AddDraggableBoxes()
        {
            foreach (var box in ViewModel.SupplierList)
            {
                SupplierElement dBox = new SupplierElement(box);
                Canvas.SetLeft(dBox, box.Position.X);
                Canvas.SetTop(dBox, box.Position.Y);

                dBox.MouseDown += Box_MouseDown;
                dBox.BoxChanged += Box_Position_Changed;
                dBox.RadialClicked += StartConnection_Click;
                dBox.RadialClicked += FinishConnection_Click;
                dBox.BoxDeleted += RemoveSupplier;

                AddBoxToTracker(dBox);

                DiagramCanvas.Children.Add(dBox);
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.updateFileSave();
        }

        private void UpdateLinePosition(ShippingLine line1, INodeElement box1, INodeElement box2)
        {
            Point startpoint;
            Point endpoint;
            if (box1.GetType() == typeof(SupplierElement))
            {
                SupplierElement b1 = (SupplierElement)box1;
                b1.CornerClicked = line1.FromJoiningBoxCorner;
                Point offset1 = GetLineOffset(b1);

                startpoint = ((SupplierElement)box1).TranslatePoint(new Point(offset1.X, offset1.Y), DiagramCanvas);
            }

            if (box2.GetType() == typeof(SupplierElement))
            {
                SupplierElement b2 = (SupplierElement)box2;
                b2.CornerClicked = line1.ToJoiningBoxCorner;
                Point offset2 = GetLineOffset(b2);

                endpoint = ((SupplierElement)box2).TranslatePoint(new Point(offset2.X, offset2.Y), DiagramCanvas);
            }
            else if (box2.GetType() == typeof(EndpointElement))
            {
                endpoint = new Point(Canvas.GetLeft(((EndpointElement)box2)) + ((EndpointElement)box2).EndpointRadial.ActualWidth,
                    Canvas.GetTop(((EndpointElement)box2)) + ((EndpointElement)box2).EndpointRadial.ActualHeight);

            }


            line1.ourShippingLine.X1 = startpoint.X;
            line1.ourShippingLine.X2 = endpoint.X;
            line1.ourShippingLine.Y1 = startpoint.Y;
            line1.ourShippingLine.Y2 = endpoint.Y;
        }

        private void AddBox_Click(object sender, RoutedEventArgs e)
        {
            AddNewBox();
        }

        private void AddNewBox()
        {
            SupplierUIValues b = new SupplierUIValues()
            {
                Position = new System.Drawing.Point(50, 50),
                supplier = new Supplier
                {
                    Id = Guid.NewGuid(),
                    Name = "New Box",
                    ProductInventory = {
                    new Product
                    {
                        Quantity = 1,
                        ProductName = "joke"
                    }
                    }
                }
            };
            SupplierElement newBox = new SupplierElement(b);
            Canvas.SetLeft(newBox, b.Position.X);
            Canvas.SetTop(newBox, b.Position.Y);
            newBox.Width = 100;
            newBox.Height = 50;
            newBox.MouseDown += Box_MouseDown;
            newBox.BoxChanged += Box_Position_Changed;
            newBox.RadialClicked += StartConnection_Click;
            newBox.RadialClicked += FinishConnection_Click;
            newBox.BoxDeleted += RemoveSupplier;

            AddBoxToTracker(newBox);

            ViewModel.AddSupplierToChain(b);
            DiagramCanvas.Children.Add(newBox);
        }

        private void AddBoxToTracker(SupplierElement box)
        {
            string boxInfo = $"Box {box.Name}: ({Canvas.GetLeft(box)}, {Canvas.GetTop(box)})";
            boxList.Add(boxInfo);
        }

        public void UpdateBoxTracker()
        {
            boxList.Clear();
            foreach (UIElement element in DiagramCanvas.Children)
            {
                if (element is SupplierElement box)
                {
                    string boxInfo = $"Box {boxList.Count + 1}: ({Canvas.GetLeft(box):F0}, {Canvas.GetTop(box):F0})";
                    boxList.Add(boxInfo);
                }
            }
        }

        private void StartConnection_Click(object sender, EventArgs e)
        {
            if (sender is SupplierElement lineTarget && MouseIsCaptured == false)
            {
                ShippingLine shippingLine = new ShippingLine();
                shippingLine.MouseDown += Line_MouseDown;
                shippingLine.ShipmentOrder.Sender = lineTarget.NodeUIValues.supplier;
                shippingLine.FromJoiningBoxCorner = lineTarget.CornerClicked;
                shippingLine.Source = lineTarget;

                Point pos = GetLineOffset(lineTarget);

                shippingLine.ourShippingLine.X1 = Canvas.GetLeft(lineTarget) + pos.X;
                shippingLine.ourShippingLine.Y1 = Canvas.GetTop(lineTarget) + pos.Y;
                //CaptureMouse();
                LogEventMessage("StartConnection_Click after Capture Mouse");
                Point mousepos = Mouse.GetPosition(DiagramCanvas);
                MouseIsCaptured = true;

                shippingLine = AssignLineValues(mousepos, shippingLine);
                targetShipingLine = shippingLine;
                targetShipment = ViewModel.ShipmentFirstSuplier((Supplier)lineTarget.NodeUIValues.supplier);
                DiagramCanvas.Children.Insert(DiagramCanvas.Children.Count, shippingLine);
            }
            if (sender is EndpointElement lineTarget_endpoint && MouseIsCaptured == false)
            {
                ShippingLine shippingLine = new ShippingLine();
                EndpointUIValues euiv = ViewModel.EndpointList.First(x => x.supplier.Id == lineTarget_endpoint.Id);
                shippingLine.ShipmentOrder.Receiver = euiv.supplier;
                shippingLine.ourShippingLine.X1 = Canvas.GetLeft(lineTarget_endpoint);
                shippingLine.ourShippingLine.Y1 = Canvas.GetTop(lineTarget_endpoint);
                shippingLine.Destination = lineTarget_endpoint;
                LogEventMessage("StartConnection_Click after Capture Mouse");
                Point mousepos = Mouse.GetPosition(DiagramCanvas);
                MouseIsCaptured = true;
                shippingLine = AssignLineValues(mousepos, shippingLine);
                targetShipingLine = shippingLine;
                DiagramCanvas.Children.Insert(DiagramCanvas.Children.Count, shippingLine);
            }
        }

        private ShippingLine AssignLineValues(Point position, ShippingLine shippingLine)
        {
            shippingLine.ourShippingLine.X2 = position.X;
            shippingLine.ourShippingLine.Y2 = position.Y;
            shippingLine.ourShippingLine.StrokeThickness = 10;
            shippingLine.ourShippingLine.Stroke = new SolidColorBrush(Colors.Red);
            Panel.SetZIndex(shippingLine, -1);
            return shippingLine;
        }

        private void LogEventMessage(string message)
        {
            System.Diagnostics.Debug.WriteLine(message);
        } 

        private void MoveConnection_MouseMove(object sender, MouseEventArgs e)
        {
            if (MouseIsCaptured && targetShipingLine is not null)
            {
                Point mousepos = e.GetPosition(this);

                //DiagramCanvas.Children.Remove(targetShipment);
                Point p1 = DiagramCanvas.TransformToAncestor(this).Transform(new Point(0, 0));
                //System.Diagnostics.Debug.WriteLine(xWidth);
                targetShipingLine.ourShippingLine.X2 = mousepos.X - p1.X;
                targetShipingLine.ourShippingLine.Y2 = mousepos.Y - p1.Y;
                //DiagramCanvas.Children.Add(targetShipment);
            }
        }

        private Point GetLineOffset(SupplierElement lineTarget)
        {
            double lineXOffset = 0;
            double lineYOffset = 0;
            switch (lineTarget.CornerClicked.ToLower())
            {
                case "n_radial":
                    {
                        lineXOffset = lineTarget.Width / 2;
                        break;
                    }
                case "ne_radial":
                    {
                        lineXOffset = lineTarget.Width;
                        break;
                    }
                case "e_radial":
                    {
                        lineXOffset = lineTarget.Width;
                        lineYOffset = lineTarget.Height / 2;
                        break;
                    }
                case "se_radial":
                    {
                        lineXOffset = lineTarget.Width;
                        lineYOffset = lineTarget.Height;
                        break;
                    }
                case "s_radial":
                    {
                        lineXOffset = lineTarget.Width / 2;
                        lineYOffset = lineTarget.Height;
                        break;
                    }
                case "sw_radial":
                    {
                        lineYOffset = lineTarget.Height;
                        break;
                    }
                case "w_radial":
                    {
                        lineYOffset = lineTarget.Height / 2;
                        break;
                    }
            }

            return new Point(lineXOffset, lineYOffset);
        }

        private void FinishConnection_Click(object sender, EventArgs e)
        {
            if (sender is SupplierElement lineTarget && MouseIsCaptured && IsDestinationSearching)
            {
                Point LineAnchorOffset = GetLineOffset(lineTarget);
                if(targetShipingLine.ShipmentOrder.Receiver is not null && targetShipingLine.ShipmentOrder.Receiver.GetType() == typeof(EndpointNode))
                {
                    targetShipingLine.ShipmentOrder.Sender = lineTarget.NodeUIValues.supplier;
                    targetShipingLine.Source = lineTarget;
                }
                else
                {
                    targetShipingLine.ShipmentOrder.Receiver = lineTarget.NodeUIValues.supplier;
                    targetShipingLine.Destination = lineTarget;
                }

                targetShipingLine.ToJoiningBoxCorner = lineTarget.CornerClicked;

                targetShipingLine.ourShippingLine.X2 = Canvas.GetLeft(lineTarget) + LineAnchorOffset.X;
                targetShipingLine.ourShippingLine.Y2 = Canvas.GetTop(lineTarget) + LineAnchorOffset.Y;
                //ReleaseMouseCapture();
                MouseIsCaptured = false;
                IsDestinationSearching = false;
                ShipmentList.Add(targetShipingLine);
                targetShipment.Receiver = lineTarget.NodeUIValues.supplier;
                ViewModel.ShipmentList.Add(targetShipment);
                targetShipment = null;
            }
            else if (sender is EndpointElement lineTarget_endpoint && MouseIsCaptured && IsDestinationSearching)
            {
                targetShipingLine.ShipmentOrder.Receiver = lineTarget_endpoint.NodeUIValues.supplier;
                double senseX2 = Canvas.GetLeft(lineTarget_endpoint) + lineTarget_endpoint.EndpointRadial.ActualWidth / 2;
                double senseY2 = Canvas.GetTop(lineTarget_endpoint) + lineTarget_endpoint.EndpointRadial.ActualHeight / 2;
                targetShipingLine.ourShippingLine.X2 = Canvas.GetLeft(lineTarget_endpoint) + lineTarget_endpoint.EndpointRadial.ActualWidth/2;
                targetShipingLine.ourShippingLine.Y2 = Canvas.GetTop(lineTarget_endpoint) + lineTarget_endpoint.EndpointRadial.ActualHeight/2;
                targetShipingLine.Destination = lineTarget_endpoint;

                MouseIsCaptured = false;
                IsDestinationSearching = false;
                ShipmentList.Add(targetShipingLine);
                //Shipment s = new Shipment();
                //s.Sender = targetShipment.Source.nodeUIValues.supplier;
                //s.Receiver = targetShipment.Source.nodeUIValues.supplier;
                //_viewModel.ShipmentList.Add(targetShipment);

            }
            else if (sender is not null && sender is not SupplierElement && IsDestinationSearching)
            {
                DiagramCanvas.Children.Remove(targetShipingLine);
                targetShipingLine = null;
                IsDestinationSearching = false;
            }
            else
            {
                IsDestinationSearching = true;
            }
        }

        private void Box_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (sender is SupplierElement selectedBox)
            {
                SetSelectedBoxDisplay(selectedBox);
                if (isRemovingConnection)
                {
                    selectedBoxForRemoval = selectedBox;
                }
                else if (isAddingConnection)
                {
                    if (firstSelectedBox == null)
                    {
                        firstSelectedBox = selectedBox;
                        SelectedBoxDetails.Text = "Select the second box to connect";
                    }
                    else if (firstSelectedBox != selectedBox)
                    {
                        isAddingConnection = false;
                        firstSelectedBox = null;
                        SelectedBoxDetails.Text = "Connection created";
                        Mouse.OverrideCursor = null;

                        var addConnectionButton = FindName("AddConnectionButton") as Button;
                        if (addConnectionButton != null)
                        {
                            addConnectionButton.IsEnabled = true;
                        }
                        else
                        {
                            SelectedBoxDetails.Text = "Cannot connect a box to itself. Select a different box.";
                        }
                    }
                }
                else
                {
                    UpdateSelectedBoxDetails(selectedBox);
                }
            }
        }

        private void UpdateSelectedBoxDetails(SupplierElement box)
        {
            Guid boxId;
            selectedElement.Item1 = box;
            Guid.TryParse(box.Name, out boxId);
            SelectedBoxDetails.Text = $"Position: ({Canvas.GetLeft(box):F0}, {Canvas.GetTop(box):F0})\n" +
                                      $"Size: {box.Width:F0}x{box.Height:F0}\n" +
                                      $"Color: {(box.boxBorder.Background as System.Windows.Media.SolidColorBrush)?.Color}\n" +
                                      $"Connected suppliers:";
            PositionTextBox.Text = $"({Canvas.GetLeft(box):F0}, {Canvas.GetTop(box):F0})";
            SizeTextBox.Text = $"{box.Width:F0}x{box.Height:F0}";
            ColorTextBox.Text = $"{(box.boxBorder.Background as System.Windows.Media.SolidColorBrush)?.Color}";
            TitleTextBox.Text = $"{box.BoxTitle.Text}";

            // Populate the ProductsListView with products from the box
            if (box.NodeUIValues?.supplier?.ProductInventory != null)
            {
                ProductsListView.ItemsSource = box.NodeUIValues.supplier.ProductInventory;
            }
            else
            {
                ProductsListView.ItemsSource = null;
            }
        }

        private void Box_Changed(object sender, EventArgs e)
        { 
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            SupplierElement box = selectedElement.Item1;

            // Edit Box title
            string title = TitleTextBox.Text.Trim();
            if (title.Length > 0)
            {
                box.BoxTitle.Text = title;
                this.Title = title;
            }
            //else
            //{
            //    MessageBox.Show("Invalid Title. Please enter a Title.");
            //    return;
            //}

            // Edit Box position
            string[] positionParts = PositionTextBox.Text.Trim('(', ')').Split(',');
            if (positionParts.Length == 2 &&
                double.TryParse(positionParts[0], out double left) &&
                double.TryParse(positionParts[1], out double top))
            {
                Canvas.SetLeft(box, left);
                Canvas.SetTop(box, top);
            }

            // Edit Box size
            string[] sizeParts = SizeTextBox.Text.Split('x');
            if (sizeParts.Length == 2 &&
                double.TryParse(sizeParts[0], out double width) &&
                double.TryParse(sizeParts[1], out double height))
            {
                box.Width = width;
                box.Height = height;
            }

            // Edit Product
            if (selectedProduct != null)
            {
                // Update ProductName
                selectedProduct.ProductName = ProductNameTextBox.Text.Trim();

                // Update Quantity
                if (float.TryParse(QuantityTextBox.Text, out float quantity))
                {
                    selectedProduct.Quantity = quantity;
                }
                else
                {
                    MessageBox.Show("Invalid quantity. Please enter a valid number.");
                    return;
                }

                // Update Units
                selectedProduct.Units = UnitsTextBox.Text.Trim();

                // Update Price
                if (decimal.TryParse(PriceTextBox.Text, out decimal price))
                {
                    selectedProduct.Price = price;
                }
                else
                {
                    MessageBox.Show("Invalid price. Please enter a valid number.");
                    return;
                }
            }

            // Edit Box color
            var colorInput = ColorTextBox.Text.Trim();
            try
            {
                var newColor = (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString(colorInput);
                box.boxBorder.Background = new System.Windows.Media.SolidColorBrush(newColor);
            }
            catch (FormatException)
            {
                MessageBox.Show("Invalid color format. Please enter a valid color (e.g., #FF0000 or Red).");
            }
        }

        private void ProductsListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            selectedProduct = (Product)ProductsListView.SelectedItem;

            if (selectedProduct != null)
            {
                ProductNameTextBox.Text = selectedProduct.ProductName;
                QuantityTextBox.Text = selectedProduct.Quantity.ToString();
                UnitsTextBox.Text = selectedProduct.Units;
                PriceTextBox.Text = selectedProduct.Price.ToString();
            }
        }
        private void Box_Position_Changed(object sender, EventArgs e)
        {
            //this gets called every time you hover over the element after selecting it...
            //Even when you select another... they both call this on hover
            UpdateBoxTracker();
            if (sender is SupplierElement changedbox)
            {
                
                UpdateSelectedBoxDetails(changedbox);

                foreach (ShippingLine sl in ShipmentList)
                {
                    UpdateLinePosition(sl, sl.Source, sl.Destination);
                }
            }
            else if(sender is EndpointElement changedbox_e)
            {
                foreach (ShippingLine sl in ShipmentList)
                {
                    UpdateLinePosition(sl, sl.Source, sl.Destination);
                }
            }
        }

        private void ResetRemoveConnectionState()
        {
            isRemovingConnection = false;
            selectedBoxForRemoval = null;
            Mouse.OverrideCursor = null;

            var removeConnectionButton = FindName("RemoveConnectionButton") as Button;
            if (removeConnectionButton != null)
            {
                removeConnectionButton.IsEnabled = true;
            }
        }

        public void RemoveSupplier(object sender, EventArgs e)
        {
            if (sender is SupplierElement selectedBox)
            {
                Guid SupplierId = selectedBox.NodeUIValues.supplier.Id;
                var supplier = ViewModel.SupplierList.FirstOrDefault(s => s.supplier.Id == SupplierId);
                if (supplier != null)
                {
                    ViewModel.SupplierList.Remove(supplier);
                }
                else
                {
                    Console.WriteLine($"no supplier found to delete with guid {SupplierId}");
                }
            }

        }

        private void AdvanceTime_Click(object sender, EventArgs e)
        {
            ViewModel.AdvanceTime();
        }

        private void AddEndpointElement_Click(object sender, RoutedEventArgs e)
        {
            MakeEndpoint();
        }

        private void MakeEndpoint()
        {
            EndpointUIValues EUIV = new EndpointUIValues();
            EUIV.SetDefaultValues();
            EndpointElement element = new EndpointElement(EUIV);
            element.Id = EUIV.supplier.Id;
            element.DataContext = ViewModel;

            element.ElementMoved += Box_Position_Changed;
            element.RadialClicked += StartConnection_Click;
            element.RadialClicked += FinishConnection_Click;
            element.ElementClicked += SelectEndpoint_Click;
            element.DataContext = ViewModel;

            Canvas.SetLeft(element, EUIV.Position.X);
            Canvas.SetTop(element, EUIV.Position.Y);
            ViewModel.AddEndpointToChain(EUIV);
            DiagramCanvas.Children.Add(element);
        }

        private void SelectEndpoint_Click(object sender, EventArgs e)
        {
            UnselectAllCanvasElements();
            if(sender is EndpointElement element)
            {
                ViewModel.SelectedEndpoint = (EndpointUIValues)element.NodeUIValues;
                EndpointProductListData = element.NodeUIValues.supplier.ProductInventory;
                EndpointProductionLineData = ((EndpointNode)element.NodeUIValues.supplier).ProductionList;
                EndpointComponentListData = ((EndpointNode)element.NodeUIValues.supplier).ComponentInventory;
                EndpointDeliveryListData = ((EndpointNode)element.NodeUIValues.supplier).DeliveryRequirementsList;
                //foreach(var item in EndpointProductListData)
                //{
                //    if(!EndpointProductList.Items.Contains(item))
                //        EndpointProductList.Items.Add(item);
                //}
                //foreach(var item in EndpointComponentListData)
                //{
                //    //EndpointComponentList.Items.Add(item);
                //}
                //foreach(var item in EndpointDeliveryListData)
                //{
                //    //EndpointComponentList.Items.Add(item);
                //}

                foreach(var item in EndpointProductionLineData)
                {
                    foreach(var component in item.Components)
                    {
                        
                    }

                }

                element.ElementBorder.BorderThickness = new Thickness(4);
                element.ElementBorder.BorderBrush = Brushes.PaleVioletRed;
                LeftSidebarEndpoint.Visibility = Visibility.Visible;
             }
         }
        private void SetSelectedBoxDisplay(SupplierElement selectedBox)
        {
            UnselectAllCanvasElements();
            selectedBox.boxBorder.BorderThickness = new Thickness(3);
            selectedBox.boxBorder.BorderBrush = Brushes.PaleVioletRed;
            LeftSidebarSupplier.Visibility = Visibility.Visible;
        }

        private void UnselectAllCanvasElements()
        {
            foreach (object Element in DiagramCanvas.Children)
            {
                if (Element is SupplierElement supplierElement)
                {
                    supplierElement.boxBorder.BorderThickness = new Thickness(1);
                    supplierElement.boxBorder.BorderBrush = Brushes.Black;
                }
                else if (Element is EndpointElement endpointElement)
                {
                    endpointElement.ElementBorder.BorderThickness = new Thickness(1);
                    endpointElement.ElementBorder.BorderBrush = Brushes.Black;
                }
                else if (Element is ShippingLine lineElement)
                {
                    lineElement.ourShippingLine.StrokeThickness = 3;
                    lineElement.ourShippingLine.Stroke = new SolidColorBrush(Colors.Black);
                }
            }
            LeftSidebarEndpoint.Visibility = Visibility.Hidden;
            LeftSidebarSupplier.Visibility = Visibility.Hidden;
            LeftSidebarLineDetails.Visibility = Visibility.Hidden;
        }

        private void AddProductToEndpoint_Click(object sender, RoutedEventArgs e)
        {
            if (ViewModel.SupplierList is not null && ViewModel.SelectedEndpoint is not null)
            {

                ((EndpointNode)ViewModel.SupplierList.First(x => x.supplier.Id == ViewModel.SelectedEndpoint.supplier.Id)
                .supplier).ProductInventory.Add(new Product()
                {
                    Price = 0,
                    Units = "",
                    ProductName = "New product",
                    Quantity = 0
                });

            }
        }
        private void AddComponentToEndpoint_Click(object sender, RoutedEventArgs e)
        {
            if(IsViewModelEndpointUsable())
            {

                ((EndpointNode)ViewModel.SupplierList.First(x => x.supplier.Id == ViewModel.SelectedEndpoint.supplier.Id)
                .supplier).ComponentInventory.Add(new Product()
                {
                    Price = 0,
                    Units = "",
                    ProductName = "New product",
                    Quantity = 0
                });
            
            }
        }

        private void AddDeliveryToEndpoint_Click(Object sender, RoutedEventArgs e)
        {
            if (IsViewModelEndpointUsable())
            {
                ((EndpointNode)ViewModel.SupplierList.First(x => x.supplier.Id == ViewModel.SelectedEndpoint.supplier.Id)
                .supplier).DeliveryRequirementsList.Add(new Product()
                {
                    Price = 0,
                    Units = "",
                    ProductName = "New product",
                    Quantity = 0
                });
            }
        }

        private bool IsViewModelEndpointUsable()
        {
            return ViewModel.SupplierList is not null && ViewModel.SelectedEndpoint is not null;
        }

        private void AddProductLineToEndpoint_Click(object sender, RoutedEventArgs e)
        {
            if(IsViewModelEndpointUsable())
            {
                ((EndpointNode)ViewModel.SupplierList.First(x => x.supplier.Id == ViewModel.SelectedEndpoint.supplier.Id)
                .supplier).ProductionList.Add(new ComponentToProductTransformer()
                {
                    Components = new ObservableCollection<Product>()
                    {
                        new Product()
                        {
                            ProductName = "new component",
                            Quantity = 0
                        }
                    },
                    ResultingProduct = new Product()
                    {
                        ProductName = "New Product",
                        Quantity = 0
                    }
                });
            }
        }

        private void AddComponentToPLEndpoint_Click(object sender, RoutedEventArgs e)
        {
            if (IsViewModelEndpointUsable())
            {
                Console.WriteLine(sender.ToString());
            }
        }

        private void SaveEndpointElementData_Click(object sender, RoutedEventArgs e)
        {
            if (ViewModel.SelectedEndpoint is null)
                return;
            ((EndpointNode)ViewModel.SelectedEndpoint.supplier).ProductInventory = EndpointProductListData;
            ((EndpointNode)ViewModel.SelectedEndpoint.supplier).ComponentInventory = EndpointComponentListData;
            ((EndpointNode)ViewModel.SelectedEndpoint.supplier).ProductionList = EndpointProductionLineData;
            ((EndpointNode)ViewModel.SelectedEndpoint.supplier).DeliveryRequirementsList = EndpointDeliveryListData;

            ViewModel.SupplierList.Where(x => x.supplier.Id == ViewModel.SelectedEndpoint.supplier.Id)
                .Select(item => item = ViewModel.SelectedEndpoint);
            foreach (var item in DiagramCanvas.Children)
            {
                if(item is EndpointElement eitem)
                {
                    if(eitem.Id == ViewModel.SelectedEndpoint.supplier.Id)
                    {
                        // Until we can get a better binding model for the dynamically created elements,
                        // We're forcibly re-updating them with the save button. 
                        // That way those elements are always up to date.
                        eitem.NodeUIValues = ViewModel.SelectedEndpoint;
                        eitem.PopulateElementLists();
                    }

                }
            };
        }

        private void Line_MouseDown(object sender, MouseButtonEventArgs e)
        {
            UnselectAllCanvasElements();
            if (sender is ShippingLine l)
            {
                ShippingLineDetailsList.ItemsSource = l.ShippingDetails;
                if (l.ShippingDetails != null)
                {
                    ShippingDetails shippingDetails = new ShippingDetails("Cow", 15.8, 4, "Livestock");
                    l.ShippingDetails.Add(shippingDetails);
                }
                l.ourShippingLine.StrokeThickness = 5;
                l.ourShippingLine.Stroke = new SolidColorBrush(Colors.PaleVioletRed);
                LeftSidebarLineDetails.Visibility = Visibility.Visible;
            }
            else
            {
                ShippingLineDetailsList.ItemsSource = null;
            }
        }

    }
}