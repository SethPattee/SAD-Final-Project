using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using FactorySADEfficiencyOptimizer.Data;
using FactorySADEfficiencyOptimizer.Shapes;
using FactorySADEfficiencyOptimizer.ViewModel;
using FactorySADEfficiencyOptimizer.Models;
using FactorySADEfficiencyOptimizer;
using System.Windows.Documents;
using System.Windows.Data;
using System.Reflection;
using FactorySADEfficiencyOptimizer.Components;
using System.Net;
using System.Xml.Linq;
using FactorySADEfficiencyOptimizer.UIComponent;
using System.Diagnostics;
using FactorSADEfficiencyOptimizer.UIComponent;
using FactorSADEfficiencyOptimizer.ViewModel;
using FactorySADEfficiencyOptimizer.UIComponent.EventArguments;

namespace YourNamespace
{
    public partial class MainWindow : Window
    {
        private ObservableCollection<string> boxList = new ObservableCollection<string>();
        public SupplyChainViewModel ViewModel { get; set; }
        private SupplierElement? selectedBoxForRemoval = null;
        private SupplierElement? firstSelectedBox = null;
        private Popup? connectionSelectionPopup;
        private bool isRemovingConnection = false;
        private bool isAddingConnection = false;
        private bool MouseIsCaptured = false;
        private bool IsDestinationSearching = false;
        private ShippingLine? targetShipingLine = null;
        private List<ShippingLine> ShipmentList = new List<ShippingLine>();
        private Product selectedProduct;
        public (SupplierElement?, EndpointElement?, Shipment?) selectedElement = new();
        public event EventHandler? BoxChanged;
        public event EventHandler? LineChanged;



    public MainWindow()
        {
            InitializeComponent();
            WindowState = WindowState.Maximized;
            ViewModel = new SupplyChainViewModel(new InitializedDataProvider());
            DataContext = ViewModel;
            Initialize();
            //sideBar.BoxList.ItemsSource = boxList;
            this.KeyDown += MainWindow_KeyDown;
            selectedProduct = new Product();
        }

        private void Initialize()
        {
            ViewModel.Load();
            AddCanvasElements();
        }
        

        private void AddCanvasElements()
        {
            foreach (var box in ViewModel.SupplierList)
            {
                if (box is SupplierUIValues)
                {
                    SupplierElement dBox = new SupplierElement(box);
                    Canvas.SetLeft(dBox, box.Position.X);
                    Canvas.SetTop(dBox, box.Position.Y);

                    dBox.MouseDown += Box_MouseDown;
                    dBox.BoxChanged += Box_Position_Changed;
                    dBox.RadialClickedTop += StartConnection_Click;
                    dBox.RadialClickedTop += FinishConnection_Click;
                    dBox.RadialClickedBottom += StartConnection_Click;
                    dBox.RadialClickedBottom += FinishConnection_Click;
                    dBox.BoxDeleted += RemoveSupplier;

                    AddBoxToTracker(dBox);

                    DiagramCanvas.Children.Add(dBox);
                }
            }
            foreach (var EUIV in ViewModel.EndpointList)
            {
				if (EUIV is EndpointUIValues)
				{
					AddEndpointToCanvas(EUIV);
				}
			}
            foreach (Shipment shipment in ViewModel.ShipmentList)
            {
				ShippingLine shippingLine = new ShippingLine();
				DiagramCanvas.Children.Insert(DiagramCanvas.Children.Count, shippingLine);
                Canvas.SetZIndex(shippingLine, -10);
                shippingLine.OwnShipment = shipment;
                shippingLine.FromJoiningBoxCorner = shipment.FromJoiningBoxCorner;
                shippingLine.ToJoiningBoxCorner = shipment.ToJoiningBoxCorner;
				shippingLine.ShippmentDeleted += RemoveShipment;
				shippingLine.LineSelected += Line_MouseDown;
                SupplierElement sender = new SupplierElement(new SupplierUIValues());
                INodeElement receiver = new SupplierElement(new SupplierUIValues());
				foreach (var child in DiagramCanvas.Children)
                {
                    if (child is SupplierElement supplierElement)
                    {
                        if (supplierElement.SupplierVM.Supplier.Id == shipment.Sender.Id)
                        {
                            sender = supplierElement;
                        }
                        else if (supplierElement.SupplierVM.Supplier.Id == shipment.Receiver.Id)
                        {
                            receiver = supplierElement;
                        }
                    }
                    else if (child is EndpointElement endpoint) {
						if (endpoint.SupplierVM.Supplier.Id == shipment.Receiver.Id)
						{
							receiver = endpoint;
						}
					}

                }
                shippingLine.Source = sender;
                shippingLine.Destination = receiver;
				Point pos = GetLineOffset(sender);
				shippingLine.ourShippingLine.X1 = Canvas.GetLeft(sender) + pos.X;
				shippingLine.ourShippingLine.Y1 = Canvas.GetTop(sender) + pos.Y;
                if (receiver is EndpointElement endpointElement)
                {
                    shippingLine.ourShippingLine.X2 = Canvas.GetLeft(endpointElement);
                    shippingLine.ourShippingLine.Y2 = Canvas.GetTop(endpointElement);
                }
                else if (receiver is SupplierElement supplierElement)
                {
                    Point LineAnchorOffset = GetLineOffset(supplierElement);
                    shippingLine.ourShippingLine.X2 = Canvas.GetLeft(supplierElement) + LineAnchorOffset.X;
                    shippingLine.ourShippingLine.Y2 = Canvas.GetTop(supplierElement) + LineAnchorOffset.Y;
                }
				ShipmentList.Add(shippingLine);
			}
            if (ViewModel.EndpointList.Count == 0)
            {
                MakeNewEndpoint();
            }
        }
        private void MakeNewEndpoint()
		{
			EndpointUIValues EUIV = new EndpointUIValues();
			EUIV.SetDefaultValues();
			AddEndpointToCanvas(EUIV);
			ViewModel.AddEndpointToChain(EUIV);
		}

		private void AddEndpointToCanvas(EndpointUIValues EUIV)
		{
			EndpointElement element = new EndpointElement(EUIV);
			element.Id = EUIV.Supplier.Id;
			element.DataContext = ViewModel;
			element.PopulateElementLists();
			element.ElementMoved += Box_Position_Changed;
			element.RadialClicked += StartConnection_Click;
			element.RadialClicked += FinishConnection_Click;
			element.ElementClicked += SelectEndpoint_Click;
            element.EndpointDeleted += RemoveEndpoint;

			Canvas.SetLeft(element, EUIV.Position.X);
			Canvas.SetTop(element, EUIV.Position.Y);
			DiagramCanvas.Children.Add(element);
		}

		private void Save_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.updateFileSave();
            FileMenuPopup.IsOpen = false;
        }

        private void UpdateLinePosition(ShippingLine line1, INodeElement? box1, INodeElement? box2)
        {
            if (box1 == null || box2 == null)
            {
                return;
            }
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
                Supplier = new Supplier
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
            newBox.RadialClickedTop += StartConnection_Click;
            newBox.RadialClickedTop += FinishConnection_Click;
            newBox.BoxDeleted += RemoveSupplier;

            AddBoxToTracker(newBox);

            ViewModel.AddSupplierToChain(b); // // // // //
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



        private void StartConnection_Click(object? sender, EventArgs? e)
        {
            if (MouseIsCaptured) return; // Prevent multiple captures

            if (IsDestinationSearching) return;
            
            if(e is RadialNameRoutedEventArgs rnr && sender is UIElement uie)
            {
                ShippingLine shippingLine = new ShippingLine();
                Point pos = GetLineOffset(uie);

                shippingLine.ourShippingLine.X1 = Canvas.GetLeft(uie) + pos.X;
                shippingLine.ourShippingLine.Y1 = Canvas.GetTop(uie) + pos.Y;


                if (sender is SupplierElement lineTarget)
                {
                    if (rnr.Radial_Name.ToLower() == "s_radial")
                    {
                        shippingLine.Source = lineTarget;
                        shippingLine.OwnShipment.Sender = lineTarget.SupplierVM.Supplier;
                        shippingLine.FromJoiningBoxCorner = lineTarget.CornerClicked;
                    } 
                    else if (rnr.Radial_Name.ToLower() == "n_radial")
                    {
                        shippingLine.Destination = lineTarget;
                        shippingLine.OwnShipment.Receiver = lineTarget.SupplierVM.Supplier;
                        shippingLine.ToJoiningBoxCorner = lineTarget.CornerClicked;
                    }

                    shippingLine = FinishSettingUpLine(shippingLine);
                }
                else if (sender is EndpointElement lineTarget_endpoint)
                {
                    EndpointUIValues euiv = ViewModel.EndpointList.First(x => x.Supplier.Id == lineTarget_endpoint.Id);
                    shippingLine.OwnShipment.Receiver = euiv.Supplier;
                    shippingLine.Destination = lineTarget_endpoint;

                    shippingLine = FinishSettingUpLine(shippingLine);
                }
            }
        }


        private ShippingLine FinishSettingUpLine(ShippingLine shippingLine)
        {
            LogEventMessage("StartConnection_Click after Capture Mouse");

            Point mousepos = Mouse.GetPosition(DiagramCanvas);
            MouseIsCaptured = true;

            shippingLine = AssignLineValues(mousepos, shippingLine);
            shippingLine.ShippmentDeleted += RemoveShipment;
            targetShipingLine = shippingLine;

            DiagramCanvas.Children.Insert(DiagramCanvas.Children.Count, shippingLine);

            return shippingLine;
        }
        private void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                ReleaseMouseCapture();
            }
        }

        private new void ReleaseMouseCapture()
        {
            if (MouseIsCaptured)
            {
                MouseIsCaptured = false;
                IsDestinationSearching =false;
                Mouse.Capture(null); 

                if (targetShipingLine != null) //save to the the shipment list so you need to get rid of that too
                {
                    DiagramCanvas.Children.Remove(targetShipingLine);
                    //RemoveLastShippingLine();
                    targetShipingLine = null;
                }
            }
        }
        private void RemoveLastShippingLine()
        {
            ShipmentList.RemoveAt(ShipmentList.Count - 1);
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

        public Point GetLineOffset(UIElement lineTarget)
        {
            double lineXOffset = 0;
            double lineYOffset = 0;
            if(lineTarget is SupplierElement s)
            {
                if(s.CornerClicked.ToLower() == "n_radial")
                {
                    lineXOffset = s.Width / 2;
                    
                }
                if(s.CornerClicked.ToLower() == "s_radial")
                {
                    lineXOffset = s.Width / 2;
                    lineYOffset = s.Height;
                }
            }
            if(lineTarget is EndpointElement e)
            {
                lineXOffset = e.RadialOffsetColumn.ActualWidth;
                lineYOffset = 18;
            }

            return new Point(lineXOffset, lineYOffset);
        }

        public void AssignSourceToTargetLine( SupplierElement se)
        {
            if (targetShipingLine == null)
                return;

            targetShipingLine.ourShippingLine.Stroke = new SolidColorBrush(Colors.Black);
            targetShipingLine.Source = se;
            targetShipingLine.OwnShipment.Sender = se.SupplierVM.Supplier;
            targetShipingLine.FromJoiningBoxCorner = "S_Radial";
        }
        
        public void AssignDestinationToTargetLine(SupplierElement se)
        {
            if (targetShipingLine == null)
                return;

            targetShipingLine.ourShippingLine.Stroke = new SolidColorBrush(Colors.Black);
            targetShipingLine.Destination = se;
            targetShipingLine.OwnShipment.Receiver = se.SupplierVM.Supplier;
        }

        public void FinishConnection_Click(object? sender, EventArgs e)
        {
            if (!MouseIsCaptured)
                return;

            if(!IsDestinationSearching)
            {
                IsDestinationSearching = true;
                return;
            }

            if(e is RadialNameRoutedEventArgs rnr)
            {
                if(rnr.Radial_Name.ToLower() == "n_radial")
                {
                    
                    var lineTarget = (SupplierElement)sender;
                    if (lineTarget == targetShipingLine.Source || lineTarget == targetShipingLine.Destination)
                        return;

                    if (targetShipingLine.Destination != null)
                        return;

                    else if(targetShipingLine.Source != null)
                    {
                        AssignDestinationToTargetLine(lineTarget);
                        targetShipingLine.ToJoiningBoxCorner = lineTarget.CornerClicked;
                    }
                }
                if(rnr.Radial_Name.ToLower() == "s_radial")
                {
                    var lineTarget = (SupplierElement)sender;
                    if (lineTarget == targetShipingLine.Source || lineTarget == targetShipingLine.Destination)
                        return;

                    if (targetShipingLine.Destination != null)
                    {
                        AssignSourceToTargetLine(lineTarget);
                        targetShipingLine.FromJoiningBoxCorner = lineTarget.CornerClicked;
                    }
                    else if (targetShipingLine.Source != null)
                    {
                        AssignDestinationToTargetLine(lineTarget);
                        targetShipingLine.ToJoiningBoxCorner = lineTarget.CornerClicked;
                    }
                }
                if (rnr.Radial_Name.ToLower() == "endpointradial")
                {
                    var lineTarget = (EndpointElement)sender;
                    if(targetShipingLine.Destination != null)
                    {
                        targetShipingLine.Source = targetShipingLine.Destination;
                        targetShipingLine.Destination = lineTarget;
                        targetShipingLine.OwnShipment.Sender = targetShipingLine.Source.SupplierVM.Supplier;
                        targetShipingLine.OwnShipment.Receiver = lineTarget.SupplierVM.Supplier;
                    }
                    else
                    {
                        targetShipingLine.Destination = lineTarget;
                        targetShipingLine.OwnShipment.Receiver = lineTarget.SupplierVM.Supplier;
                    }
                }

                FinnishLineMaker();
            }


   //         if (sender is SupplierElement lineTarget && MouseIsCaptured && IsDestinationSearching)
			//{
			//	Point LineAnchorOffset = GetLineOffset(lineTarget);
			//	if (targetShipingLine.OwnShipment.Receiver is not null && targetShipingLine.OwnShipment.Receiver.GetType() == typeof(EndpointNode))
			//	{
					
			//	}
			//	else
			//	{
			//		targetShipingLine.OwnShipment.Receiver = lineTarget.NodeUIValues.supplier;
			//		targetShipingLine.Destination = lineTarget;
			//	}

			//	targetShipingLine.ToJoiningBoxCorner = lineTarget.CornerClicked;
			//	if (targetShipingLine.FromJoiningBoxCorner is null)
			//	{
			//		targetShipingLine.FromJoiningBoxCorner = lineTarget.CornerClicked;
			//	}

			//	targetShipingLine.ourShippingLine.X2 = Canvas.GetLeft(lineTarget) + LineAnchorOffset.X;
			//	targetShipingLine.ourShippingLine.Y2 = Canvas.GetTop(lineTarget) + LineAnchorOffset.Y;
			//	FinnishLineMaker();
			//}
			//else if ()
   //         {
   //             //targetShipingLine.OwnShipment.Receiver = lineTarget_endpoint.NodeUIValues.supplier;
   //             //double senseX2 = Canvas.GetLeft(lineTarget_endpoint) + lineTarget_endpoint.EndpointRadial.ActualWidth / 2;
   //             //double senseY2 = Canvas.GetTop(lineTarget_endpoint) + lineTarget_endpoint.EndpointRadial.ActualHeight / 2;
   //             //targetShipingLine.ourShippingLine.X2 = Canvas.GetLeft(lineTarget_endpoint) + lineTarget_endpoint.EndpointRadial.ActualWidth/2;
   //             //targetShipingLine.ourShippingLine.Y2 = Canvas.GetTop(lineTarget_endpoint) + lineTarget_endpoint.EndpointRadial.ActualHeight/2;
   //             //targetShipingLine.Destination = lineTarget_endpoint;

			//	FinnishLineMaker();
			//}
			//else if (sender is not null && sender is not SupplierElement && IsDestinationSearching)
   //         {
   //             DiagramCanvas.Children.Remove(targetShipingLine);
   //             targetShipingLine = null;
   //             IsDestinationSearching = false;
   //         }
            //else
            //{
            //    IsDestinationSearching = true;
            //}
        }
		private void FinnishLineMaker()
		{
			MouseIsCaptured = false;
			IsDestinationSearching = false;
            if (targetShipingLine is not null)
            {
			    targetShipingLine.LineSelected += Line_MouseDown;
                targetShipingLine.OwnShipment.FromJoiningBoxCorner = targetShipingLine.FromJoiningBoxCorner;
                targetShipingLine.OwnShipment.ToJoiningBoxCorner = targetShipingLine.ToJoiningBoxCorner;
                targetShipingLine.ourShippingLine.StrokeThickness = 6;
			    ViewModel.ShipmentList.Add(targetShipingLine.OwnShipment);
			    ShipmentList.Add(targetShipingLine);

            }
		}

		private void Box_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (sender is SupplierElement selectedBox)
            {
                ViewModel.SelectedSupplier = selectedBox.SupplierVM;
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
                    }
                    else if (firstSelectedBox != selectedBox)
                    {
                        isAddingConnection = false;
                        firstSelectedBox = null;
                        Mouse.OverrideCursor = null;

                        var addConnectionButton = FindName("AddConnectionButton") as Button;
                        if (addConnectionButton != null)
                        {
                            addConnectionButton.IsEnabled = true;
                        }
                        else
                        {
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
        }

        private void Box_Changed(object sender, EventArgs e)
        { 
        }

        private void SaveSupplierDetails(object sender, EventArgs e)
        {
            if(e is SaveSupplierEventArgs s)
            {
                try
                {
                    ViewModel.SupplierList.FirstOrDefault(x => x.Supplier.Id == s.supplier!.Id)!.Supplier = s.supplier!;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }

        private void OpenSupplierInventory_Click(object sender, RoutedEventArgs e)
        {
            if(sender is Button s)
            {
                SupplierInventoryWindow DetailsWindow = new SupplierInventoryWindow().WithViewModel(((Supplier)((SupplyChainViewModel)s.DataContext).SelectedSupplier.Supplier));
                DetailsWindow.Owner = this;
                DetailsWindow.SaveSupplierHandler += SaveSupplierDetails;
                DetailsWindow.Show();
            }

        }

        //private void EditButton_Click(object sender, RoutedEventArgs e)
        //{
        //    SupplierElement box = selectedElement.Item1;

        //    // Edit Box title
        //    string title = TitleTextBox.Text.Trim();
        //    if (title.Length > 0)
        //    {
        //        box.BoxTitle.Text = title;
        //        this.Title = title;
        //    }
        //    //else
        //    //{
        //    //    MessageBox.Show("Invalid Title. Please enter a Title.");
        //    //    return;
        //    //}


        //    // Edit Product
        //    if (selectedProduct != null)
        //    {
        //        // Update ProductName

        //        // Update Quantity
        //        if (float.TryParse(QuantityTextBox.Text, out float quantity))
        //        {
        //            selectedProduct.Quantity = quantity;
        //        }
        //        else
        //        {
        //            MessageBox.Show("Invalid quantity. Please enter a valid number.");
        //            return;
        //        }

        //        // Update Units
        //        selectedProduct.Units = UnitsTextBox.Text.Trim();

        //        // Update Price
        //        if (decimal.TryParse(PriceTextBox.Text, out decimal price))
        //        {
        //            selectedProduct.Price = price;
        //        }
        //        else
        //        {
        //            MessageBox.Show("Invalid price. Please enter a valid number.");
        //            return;
        //        }
        //    }

        //    box.FillProductDisplay();
        //}

        private void ProductsListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
        }
        private void Box_Position_Changed(object? sender, EventArgs e)
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
        public void RemoveShipment(object? sender, EventArgs? e)
        {
            if (sender is ShippingLine element)
            {
                Guid shipId = element.OwnShipment.Id;
                var shipment = ViewModel.ShipmentList.FirstOrDefault(x => x.Id == shipId);
                if (shipment != null) {
					ViewModel.ShipmentList.Remove(shipment);
				}
            }
        }
        public void RemoveEndpoint(object? sender, EventArgs? e)
        {
            if (sender is EndpointElement element)
            {
				Guid EndpointId = element.SupplierVM.Supplier.Id;
				var Endpoint = ViewModel.EndpointList.FirstOrDefault(s => s.Supplier.Id == EndpointId);
                if (Endpoint != null) {
					List<Shipment> Shippments = new List<Shipment>();
					List<ShippingLine> lines = new();
					foreach (var s in ViewModel.ShipmentList)
					{
						if (s.Sender == Endpoint.Supplier || s.Receiver == Endpoint.Supplier)
						{
							Shippments.Add(s);
							foreach (var child in DiagramCanvas.Children)
							{
								if (child is ShippingLine l)
								{
									if (l.OwnShipment.Sender == Endpoint.Supplier || l.OwnShipment.Receiver == Endpoint.Supplier)
									{ lines.Add(l); }
								}
							}
						}
					}
				    ViewModel.EndpointList.Remove(Endpoint);
					foreach (var s in Shippments)
					{
						ViewModel.ShipmentList.Remove(s);
					}
					foreach (var line in lines)
					{
						DiagramCanvas.Children.Remove(line);
					}
				}
			}
        }
        public void RemoveSupplier(object? sender, EventArgs e)
        {
            if (sender is SupplierElement selectedBox)
            {
                Guid SupplierId = selectedBox.SupplierVM.Supplier.Id;
                var supplier = ViewModel.SupplierList.FirstOrDefault(s => s.Supplier.Id == SupplierId);
                if (supplier != null)
                {
                    List<Shipment> Shippments = new List<Shipment>();
                    List<ShippingLine> lines = new();
                    foreach (var s in ViewModel.ShipmentList)
                    {
                        if (s.Sender == supplier.Supplier || s.Receiver == supplier.Supplier)
                        {
                            Shippments.Add(s);
                            foreach (var child in DiagramCanvas.Children)
                            {
                                if (child is ShippingLine l)
                                {
                                    if (l.OwnShipment.Sender == supplier.Supplier || l.OwnShipment.Receiver == supplier.Supplier)
                                    { lines.Add(l); }
                                }
                            }
                        }
                    }
                    ViewModel.SupplierList.Remove(supplier);
                    foreach (var s in Shippments)
                    {
                        ViewModel.ShipmentList.Remove(s);
                    }
                    foreach (var line in lines)
                    {
                        DiagramCanvas.Children.Remove(line);
                    }
                }
                else
                {
                    Console.WriteLine($"No supplier found to delete with guid {SupplierId}");
                }
            }

        }
        
        private void AdvanceTime_Click(object sender, EventArgs e)
        {
            ViewModel.AdvanceTime();
            RefreshEndpointElementDisplay();
        }

        private void AddEndpointElement_Click(object sender, RoutedEventArgs e)
        {
            MakeNewEndpoint();
        }


        private void SelectEndpoint_Click(object? sender, EventArgs e)
        {
            UnselectAllCanvasElements();
            if(sender is EndpointElement element)
            {
                ViewModel.SelectedEndpoint = (EndpointUIValues)element.SupplierVM;
                element.ElementBorder.BorderThickness = new Thickness(4);
                element.ElementBorder.BorderBrush = Brushes.PaleVioletRed;
                LeftSidebarEndpoint.Visibility = Visibility.Visible;
                LeftSidebarScrollEndpoints.Visibility = Visibility.Visible;
             }
         }
        private void SetSelectedBoxDisplay(SupplierElement selectedBox)
        {
            UnselectAllCanvasElements();
            selectedBox.boxBorder.BorderThickness = new Thickness(3);
            selectedBox.boxBorder.BorderBrush = Brushes.PaleVioletRed;
            LeftSidebarSupplier.Visibility = Visibility.Visible;
            LeftSidebarScrollSuppliers.Visibility = Visibility.Visible;
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
                    lineElement.ourShippingLine.StrokeThickness = 6;
                    lineElement.ourShippingLine.Stroke = new SolidColorBrush(Colors.Black);
                }
            }
            LeftSidebarEndpoint.Visibility = Visibility.Collapsed;
            LeftSidebarSupplier.Visibility = Visibility.Collapsed;
            LeftSidebarLineDetails.Visibility = Visibility.Collapsed;
        }

        private void AddProductToEndpoint_Click(object sender, RoutedEventArgs e)
        {
            if (ViewModel.SupplierList is not null && ViewModel.SelectedEndpoint is not null)
            {
                ((EndpointNode)ViewModel.EndpointList.First(x => x.Supplier.Id == ViewModel.SelectedEndpoint.Supplier.Id)
                .Supplier).ProductInventory.Add(CreateNewProduct());
            }
        }
        private void AddProductToShipment_Click(object sender, RoutedEventArgs e)
        {
            if (ViewModel.ShipmentList is not null && ViewModel.SelectedShipment is not null)
            {
                ViewModel.SelectedShipment.Products.Add(new Product()
                {
                    Price = 0,
                    Units = "",
                    ProductName = "New product",
                    Quantity = 0
                });
                //This is forcing a redraw of the products in the left side bar...
                var save = ViewModel.SelectedShipment;
                ViewModel.SelectedShipment = null;
                ViewModel.SelectedShipment = save;
                // it wasn't working another way, feel free to fix, please don't break it just because it ugly
            }
        }
        private void AddComponentToEndpoint_Click(object sender, RoutedEventArgs e)
        {
            if(IsViewModelEndpointUsable())
            {
                ((EndpointNode)ViewModel.EndpointList.First(x => x.Supplier.Id == ViewModel.SelectedEndpoint.Supplier.Id)
                .Supplier).ComponentInventory.Add(CreateNewProduct());
            }
        }

        private void AddDeliveryToEndpoint_Click(Object sender, RoutedEventArgs e)
        {
            if (IsViewModelEndpointUsable())
            {
                ((EndpointNode)ViewModel.EndpointList.First(x => x.Supplier.Id == ViewModel.SelectedEndpoint.Supplier.Id)
                .Supplier).DeliveryRequirementsList.Add(CreateNewProduct());
            }
        }

        private bool IsViewModelEndpointUsable()
        {
            return ViewModel.EndpointList is not null && ViewModel.SelectedEndpoint is not null;
        }

        private void AddProductLineToEndpoint_Click(object sender, RoutedEventArgs e)
        {
            if(IsViewModelEndpointUsable())
            {
                ((EndpointNode)ViewModel.EndpointList.First(x => x.Supplier.Id == ViewModel.SelectedEndpoint.Supplier.Id)
                .Supplier).ProductionList.Add(new ProductLine()
                {
                    Components = new ObservableCollection<Product>()
                    {
                        CreateNewProduct()
                    },
                    ResultingProduct = CreateNewProduct()
                });
            }
        }

        private void AddComponentToPLEndpoint_Click(object sender, RoutedEventArgs e)
        {
            if (IsViewModelEndpointUsable() && sender is Button button)
            {
                ((ProductLine)button.DataContext).Components.Add(CreateNewProduct());
            }
        }


        private Product CreateNewProduct()
        {
            Product product = new Product()
            {
                Price = 0,
                Units = "",
                ProductName = "New product",
                Quantity = 0
            };

            return product;
        }

        private void SaveEndpointElementData_Click(object sender, RoutedEventArgs e)
        {
            RefreshEndpointElementDisplay();
        }

        private void RefreshEndpointElementDisplay()
        {
            if (ViewModel.SelectedEndpoint is null)
                return;
            foreach (var item in DiagramCanvas.Children)
            {
                if (item is EndpointElement eitem)
                {
                    if (eitem.SupplierVM.Supplier.Id == ViewModel.SelectedEndpoint.Supplier.Id)
                    {
                        // Until we can get a better binding model for the dynamically created elements,
                        // We're forcibly re-updating them with the save button. 
                        // That way those elements are always up to date.
                        eitem.PopulateElementLists();
                    }

                }
            };
        }

        private void Line_MouseDown(object? sender, EventArgs? e)
        {
            UnselectAllCanvasElements();
            if (sender is ShippingLine l)
            {
                SetSelectedShippingLine(l);
            }
        }

        private void SetSelectedShippingLine(ShippingLine l)
        {
            ViewModel.SelectedShipment = l.OwnShipment;
            l.ourShippingLine.StrokeThickness = 8;
            l.ourShippingLine.Stroke = new SolidColorBrush(Colors.PaleVioletRed);
            LeftSidebarLineDetails.Visibility = Visibility.Visible;
            LeftSidebarScrollShipments.Visibility = Visibility.Visible;
        }

        private void LineSelectedFromWindow_Click(object? sender, EventArgs e)
        {
            var selected = ((SelectedShipmentWindowMouseButtonEventArgs)e).selected;
            if (ViewModel.ShipmentList.Any(x => x.Sender.Name == selected.Sender.Name && x.Receiver.Name == selected.Receiver.Name))
            {
                ViewModel.SelectedShipment = ViewModel.ShipmentList
                                                      .Where(x => x.Sender.Name == selected.Sender.Name
                                                              && x.Receiver.Name == selected.Receiver.Name).FirstOrDefault();

                foreach (var line in DiagramCanvas.Children.OfType<ShippingLine>())
                {
                    if(line.OwnShipment == ViewModel.SelectedShipment)
                    {
                        UnselectAllCanvasElements();
                        SetSelectedShippingLine(line);
                    }
                }
            }
        }

        private void DeleteComponentButton_Click(object sender, RoutedEventArgs e)
        {
            if(IsViewModelEndpointUsable() && sender is Button button)
            {
                var plcontexttarget = GetAncestorOfType(VisualTreeHelper.GetParent(button));

                ((ProductLine)plcontexttarget.DataContext)
                    .Components.Remove((Product)button.DataContext);

            }
        }

        private void DeleteProductLineButton_Click(object sender, RoutedEventArgs e)
        {
            if(IsViewModelEndpointUsable() && sender is Button button && ViewModel.SelectedEndpoint != null)
            {
                ((EndpointNode)ViewModel.SelectedEndpoint.Supplier)
                    .ProductionList.Remove((ProductLine)button.DataContext);
            }
        }

        private ListView GetAncestorOfType(DependencyObject element)
        {
            var parent = VisualTreeHelper.GetParent(element);
            if (parent is null)
                return null;

            if(parent.GetType() == typeof(ListView))
                return (ListView)parent;
            else
            {
                return (ListView)GetAncestorOfType((DependencyObject)parent);
            }
        }

        // Code learned from Window Ownership section of https://learn.microsoft.com/en-us/dotnet/desktop/wpf/windows/?view=netdesktop-9.0
        private void Button_ClickForAnalyze(object sender, RoutedEventArgs e)
        {
            var AnalysisWindow = new ProductionAnalysisWindow(ViewModel);
            AnalysisWindow.Owner = this;
            AnalysisWindow.simModel = new AnalizorModel(ViewModel);
            AnalysisWindow.DataContext = AnalysisWindow.simModel;
            AnalysisWindow.ShipmentHighlightPassoverHandler += LineSelectedFromWindow_Click;
            AnalysisWindow.Show();
        }
        private void Button_ClickForBillofMaterials(object sender, RoutedEventArgs e)
        {
            var BillWindow = new BillofMaterials();
            BillWindow.Owner = this;
            BillWindow.Show();
        }


        private void IncrementShipmentDeliveryTime_Click(object sender, RoutedEventArgs e)
        {
            if(ViewModel is not null && ViewModel.SelectedShipment is not null)
                ViewModel.SelectedShipment.TimeToDeliver++;
        }
        private void DecrementShipmentDeliveryTime_Click(object sender, RoutedEventArgs e)
        {
            if (ViewModel is null || ViewModel?.SelectedShipment is null)
                return;

            else if (ViewModel.SelectedShipment.TimeToDeliver - 1 <= 0)
                ViewModel.SelectedShipment.TimeToDeliver = 1;
            else
                ViewModel.SelectedShipment.TimeToDeliver--;
        }

        private void DeleteItemButton_Click(object sender, RoutedEventArgs e)
        {
            if(sender is Button button)
            {
                ViewModel.SelectedShipment?.Products
                    .Remove((Product)button.DataContext);
            }
        }

        private void DeleteProductButton_Click(object sender, RoutedEventArgs e)
        {
            if(sender is Button b)
            {
                ViewModel.SelectedEndpoint.Supplier
                    .ProductInventory.Remove((Product)b.DataContext);
            }
        }

        private void DeleteDeliveryButton_Click(object sender, RoutedEventArgs e)
        {
            if(sender is Button b && ViewModel.SelectedEndpoint != null)
            {
                ((EndpointNode)ViewModel.SelectedEndpoint.Supplier)
                    .DeliveryRequirementsList.Remove((Product)b.DataContext);
            }
        }

        private void DeleteEndpointComponentInventoryButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button b)
            {
                ViewModel.SelectedEndpoint.Supplier
                    .ComponentInventory.Remove((Product)b.DataContext);
            }
        }

        private void ClrPcker_Background_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            foreach(var endpoint in DiagramCanvas.Children.OfType<EndpointElement>())
            {
                endpoint.SetBackgroundColor(e.NewValue ?? throw new Exception("Color was null in main window color picker for Endpoint! Ya dingus!"));
            }
            
        }

        private void ClrPckr_Background_Supplier_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            DiagramCanvas.Children
                .OfType<SupplierElement>()
                .FirstOrDefault(x => x.SupplierVM.Supplier.Id == ViewModel.SelectedSupplier.Supplier.Id)
                .SetBackgroundColor(e.NewValue);


        }
    }
}