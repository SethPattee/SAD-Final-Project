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

namespace YourNamespace
{
    public partial class MainWindow : Window
    {
        private ObservableCollection<string> boxList = new ObservableCollection<string>();
        private SupplierViewModel _viewModel;
        private bool isRemovingConnection = false;
        private DraggableBox selectedBoxForRemoval = null;
        private Popup? connectionSelectionPopup;
        private bool isAddingConnection = false;
        private DraggableBox firstSelectedBox = null;
        private bool MouseIsCaptured = false;
        private bool IsDestinationSearching = false;
        private ShippingLine? targetShipment = null;
        private List<DraggableBox> SupplierList = new List<DraggableBox>();
        private List<ShippingLine> ShipmentList = new List<ShippingLine>();

        public event EventHandler? BoxChanged;
        public event EventHandler? LineChanged;

        public MainWindow()
        {
            //btcEvent.BoxClicked += StartConnection;
            InitializeComponent();
            _viewModel = new SupplierViewModel(new BoxDataProvider());
            DataContext = _viewModel;
            Loaded += BoxView_Loaded;
            BoxList.ItemsSource = boxList;
            Initialize();
        }

        private async void Initialize()
        {
            await _viewModel.LoadAsync();
            AddDraggableBoxes();
        }

        private async void BoxView_Loaded(object sender, RoutedEventArgs e)
        {
            await _viewModel.LoadAsync();
        }

        private void AddDraggableBoxes()
        {
            List<DraggableBox> draggableBoxes = new List<DraggableBox>();
            foreach (var box in _viewModel.Boxes)
            {
                DraggableBox dBox = new DraggableBox(box);
                DiagramCanvas.Children.Add(dBox);
                Canvas.SetLeft(dBox, box.xPosition);
                Canvas.SetTop(dBox, box.yPosition);
                AddBoxToTracker(dBox);

                dBox.MouseDown += Box_MouseDown;
                dBox.BoxChanged += Box_Changed;
                dBox.RadialClicked += StartConnection_Click;
                dBox.RadialClicked += FinishConnection_Click;


                draggableBoxes.Add(dBox);
            }

            AddShippingLine(draggableBoxes[0]);

            for (int i = 0; i < draggableBoxes.Count - 1; i++)
            {
                CreateConnectionBetweenBoxes(draggableBoxes[i], draggableBoxes[i + 1]);
            }
        }

        //private void AssignRadialEvents(ref DraggableBox dBox)
        //{
        //    dBox.NW_Radial.MouseDown += StartConnection_Click;
        //    dBox.N_Radial.MouseDown += StartConnection_Click;
        //    dBox.NE_Radial.MouseDown += StartConnection_Click;
        //    dBox.SE_Radial.MouseDown += StartConnection_Click;
        //    dBox.SW_Radial.MouseDown += StartConnection_Click;
        //    dBox.S_Radial.MouseDown += StartConnection_Click;
        //    dBox.E_Radial.MouseDown += StartConnection_Click;
        //    dBox.W_Radial.MouseDown += StartConnection_Click;
        //}

        private void AddShippingLine(DraggableBox box)
        {
            ShippingLine ourLine = new ShippingLine();
            ourLine.ourShippingLine.X1 = 
            DiagramCanvas.Children.Add(ourLine);
        }

        private void CreateConnectionBetweenBoxes(DraggableBox box1, DraggableBox box2)
        {
            if (box1.Connections.Any(c => c.ConnectedBox == box2) || box2.Connections.Any(c => c.ConnectedBox == box1))
            {
                SelectedBoxDetails.Text = "Connection already exists between these boxes.";
                return;
            }
            Line line = new Line
            {
                Stroke = System.Windows.Media.Brushes.Black,
                StrokeThickness = 2
            };

            DiagramCanvas.Children.Add(line);

            box1.AddConnection(box2, line);
            box2.AddConnection(box1, line);

            UpdateLinePosition(line, box1, box2);
            UpdateSelectedBoxDetails(box1);
        }

        private void AddConnection_Click(object sender, RoutedEventArgs e)
        {
            if (!isAddingConnection)
            {
                isAddingConnection = true;
                firstSelectedBox = null;
                SelectedBoxDetails.Text = "Select the first box to connect";

                Mouse.OverrideCursor = Cursors.Hand;

                (sender as Button).IsEnabled = false;
            }
        }

        private void UpdateLinePosition(Line line, DraggableBox box1, DraggableBox box2)
        {
            Point startPoint = box1.TranslatePoint(new Point(box1.ActualWidth / 2, box1.ActualHeight / 2), DiagramCanvas);
            Point endPoint = box2.TranslatePoint(new Point(box2.ActualWidth / 2, box2.ActualHeight / 2), DiagramCanvas);

            line.X1 = startPoint.X;
            line.Y1 = startPoint.Y;
            line.X2 = endPoint.X;
            line.Y2 = endPoint.Y;
        }

        private void AddBox_Click(object sender, RoutedEventArgs e)
        {
            AddNewBox();
        }

        private void AddNewBox()
        {
            BoxValues b = new BoxValues() { xPosition = 50,
                                            yPosition = 50,
                                            supplier = new Supplier
                                            {
                                                Name = "New Box",
                                                Products = {
                                                new Product
                                                {
                                                    Quantity = 1,
                                                    ProductName = "joke"
                                                }
                                                }
                                            } };
            DraggableBox newBox = new DraggableBox(b);
            Canvas.SetLeft(newBox, b.xPosition);
            Canvas.SetTop(newBox, b.yPosition);
            newBox.Width = 100;
            newBox.Height = 50;
            

            AddBoxToTracker(newBox);

            newBox.MouseDown += Box_MouseDown;
            newBox.BoxChanged += Box_Changed;
            newBox.RadialClicked += StartConnection_Click;
            newBox.RadialClicked += FinishConnection_Click;

            SupplierList.Add(newBox);
            DiagramCanvas.Children.Add(newBox);
        }

        private void AddBoxToTracker(DraggableBox box)
        {
            string boxInfo = $"Box {boxList.Count + 1}: ({Canvas.GetLeft(box)}, {Canvas.GetTop(box)})";
            boxList.Add(boxInfo);
        }

        public void UpdateBoxTracker()
        {
            boxList.Clear();
            foreach (UIElement element in DiagramCanvas.Children)
            {
                if (element is DraggableBox box)
                {
                    string boxInfo = $"Box {boxList.Count + 1}: ({Canvas.GetLeft(box):F0}, {Canvas.GetTop(box):F0})";
                    boxList.Add(boxInfo);
                }
            }
        }

        private void StartConnection_Click(object sender, EventArgs e)
        {
            if(sender is DraggableBox lineTarget && MouseIsCaptured == false)
            {
                ShippingLine shippingLine = new ShippingLine();

                Point pos = GetLineOffset(lineTarget);

                shippingLine.ourShippingLine.X1 = Canvas.GetLeft(lineTarget)+pos.X;
                shippingLine.ourShippingLine.Y1 = Canvas.GetTop(lineTarget)+pos.Y;
                //CaptureMouse();
                System.Diagnostics.Debug.WriteLine("StartConnection_Click after Capture Mouse");
                Point mousepos = Mouse.GetPosition(DiagramCanvas);
                MouseIsCaptured = true;

                shippingLine.ourShippingLine.X2 = mousepos.X;
                shippingLine.ourShippingLine.Y2 = mousepos.Y;
                shippingLine.ourShippingLine.StrokeThickness = 10;
                shippingLine.ourShippingLine.Stroke = new SolidColorBrush(Colors.Red);
                Panel.SetZIndex(shippingLine, -1);
                targetShipment = shippingLine;
                DiagramCanvas.Children.Insert(0, shippingLine);
            }
        }

        private void MoveConnection_MouseMove(object sender, MouseEventArgs e)
        {
            if(MouseIsCaptured && targetShipment is not null)
            {
                Point mousepos = e.GetPosition(this);

                DiagramCanvas.Children.Remove(targetShipment);
                double xWidth = MainGrid.ColumnDefinitions.First().ActualWidth;
                //System.Diagnostics.Debug.WriteLine(xWidth);
                //targetShipment.ourShippingLine.X2 = mousepos.X - xWidth;
                targetShipment.ourShippingLine.X2 = mousepos.X - 255;
                targetShipment.ourShippingLine.Y2 = mousepos.Y - 37;
                DiagramCanvas.Children.Add(targetShipment);
            }
        }

        private Point GetLineOffset(DraggableBox lineTarget)
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
                        lineXOffset = lineTarget.Height / 2;
                        break;
                    }
                case "se_radial":
                    {
                        lineXOffset = lineTarget.Width;
                        lineXOffset = lineTarget.Height;
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
            if(sender is DraggableBox lineTarget && MouseIsCaptured && IsDestinationSearching)
            {
                Point pos = GetLineOffset(lineTarget);

                targetShipment.ourShippingLine.X2 = Canvas.GetLeft(lineTarget)+pos.X;
                targetShipment.ourShippingLine.Y2 = Canvas.GetTop(lineTarget)+pos.Y;
                //ReleaseMouseCapture();
                MouseIsCaptured = false;
                IsDestinationSearching = false;
            }
            else if(sender is not null && sender is not DraggableBox && IsDestinationSearching)
            {
                DiagramCanvas.Children.Remove(targetShipment);
                targetShipment = null;
                IsDestinationSearching=false;
            }
            else
            {
                IsDestinationSearching = true;
            }
        }

        private void Box_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (sender is DraggableBox selectedBox)
            {
                if (isRemovingConnection)
                {
                    selectedBoxForRemoval = selectedBox;
                    ShowConnectionSelectionPopup(selectedBox);
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
                        CreateConnectionBetweenBoxes(firstSelectedBox, selectedBox);

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

        private void UpdateSelectedBoxDetails(DraggableBox box)
        {
            SelectedBoxDetails.Text = $"Position: ({Canvas.GetLeft(box):F0}, {Canvas.GetTop(box):F0})\n" +
                                      $"Size: {box.Width:F0}x{box.Height:F0}\n" +
                                      $"Color: {(box.boxBorder.Background as System.Windows.Media.SolidColorBrush)?.Color}\n" +
                                      $"Connected Boxes: {box.Connections.Count}";

            var connectedProperties = box.GetConnectedBoxProperties();
            foreach (var prop in connectedProperties)
            {
                SelectedBoxDetails.Text += $"\nConnected Box: {prop.Title}\n" +
                                           $"  Items: {string.Join(", ", prop.Items)}\n" +
                                           $"  Connected Boxes: {prop.ConnectedBoxes}";
            }
        }

        private void Box_Changed(object sender, EventArgs e)
        {
            UpdateBoxTracker();
            if (sender is DraggableBox changedBox)
            {
                UpdateSelectedBoxDetails(changedBox);

                foreach (var connection in changedBox.Connections)
                {
                    UpdateLinePosition(connection.ConnectionLine, changedBox, connection.ConnectedBox);
                }
            }
        }
        private void RemoveConnection_Click(object sender, RoutedEventArgs e)
        {
            if (!isRemovingConnection)
            {
                isRemovingConnection = true;
                selectedBoxForRemoval = null;
                SelectedBoxDetails.Text = "Select a box to remove a connection from";

                Mouse.OverrideCursor = Cursors.Hand;

                (sender as Button).IsEnabled = false;
            }
        }

        private void ShowConnectionSelectionPopup(DraggableBox box)
        {
            if (box.Connections.Count == 0)
            {
                SelectedBoxDetails.Text = "This box has no connections to remove.";
                ResetRemoveConnectionState();
                return;
            }

            connectionSelectionPopup = new Popup
            {
                IsOpen = true,
                StaysOpen = false,
                PlacementTarget = this,
                Placement = PlacementMode.Center
            };

            var border = new Border
            {
                Background = Brushes.White,
                BorderBrush = Brushes.Black,
                BorderThickness = new Thickness(1),
                Padding = new Thickness(5)
            };

            var stackPanel = new StackPanel();

            foreach (var connection in box.Connections)
            {
                var button = new Button
                {
                    Content = $"Connection to {connection.ConnectedBox.BoxTitle.Text}",
                    Margin = new Thickness(5),
                    Padding = new Thickness(5)
                };
                button.Click += (sender, e) => RemoveSelectedConnection(box, connection);
                stackPanel.Children.Add(button);
            }

            border.Child = stackPanel;
            connectionSelectionPopup.Child = border;
        }


        private void RemoveSelectedConnection(DraggableBox box, BoxConnection connectionToRemove)
        {
            box.RemoveConnection(connectionToRemove.ConnectedBox);
            connectionToRemove.ConnectedBox.RemoveConnection(box);

            DiagramCanvas.Children.Remove(connectionToRemove.ConnectionLine);

            connectionSelectionPopup.IsOpen = false;

            SelectedBoxDetails.Text = "Connection removed";
            UpdateSelectedBoxDetails(box);

            ResetRemoveConnectionState();
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
    }
}