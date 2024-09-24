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
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using WPFTesting.Models;

namespace YourNamespace
{
    public partial class MainWindow : Window
    {
        private ObservableCollection<string> boxList = new ObservableCollection<string>();
        private SupplierViewModel _viewModel;
        private bool isRemovingConnection = false;
        private DraggableBox selectedBoxForRemoval = null;
        private Popup connectionSelectionPopup;
        private bool isAddingConnection = false;
        private DraggableBox firstSelectedBox = null;

        public event EventHandler BoxChanged;

        public MainWindow()
        {
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
                InfiniteCanvas.Children.Add(dBox);
                Canvas.SetLeft(dBox, box.xPosition);
                Canvas.SetTop(dBox, box.yPosition);
                AddBoxToTracker(dBox);

                // Attach event handlers
                dBox.MouseDown += Box_MouseDown;
                dBox.BoxChanged += Box_Changed;

                draggableBoxes.Add(dBox);
            }

            // Create connections between boxes (for demonstration, connecting consecutive boxes)
            for (int i = 0; i < draggableBoxes.Count - 1; i++)
            {
                CreateConnectionBetweenBoxes(draggableBoxes[i], draggableBoxes[i + 1]);
            }
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

            // Add the line to the canvas
            InfiniteCanvas.Children.Add(line);

            // Add the connection to both boxes
            box1.AddConnection(box2, line);
            box2.AddConnection(box1, line);

            // Update the line position
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

                // Change cursor to indicate selection mode
                Mouse.OverrideCursor = Cursors.Hand;

                // Disable the Add Connection button while in selection mode
                (sender as Button).IsEnabled = false;
            }
        }

        private void UpdateLinePosition(Line line, DraggableBox box1, DraggableBox box2)
        {
            Point startPoint = box1.TranslatePoint(new Point(box1.ActualWidth / 2, box1.ActualHeight / 2), InfiniteCanvas);
            Point endPoint = box2.TranslatePoint(new Point(box2.ActualWidth / 2, box2.ActualHeight / 2), InfiniteCanvas);

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
            InfiniteCanvas.Children.Add(newBox);

            AddBoxToTracker(newBox);

            newBox.MouseDown += Box_MouseDown;
            newBox.BoxChanged += Box_Changed;
        }

        private void AddBoxToTracker(DraggableBox box)
        {
            string boxInfo = $"Box {boxList.Count + 1}: ({Canvas.GetLeft(box)}, {Canvas.GetTop(box)})";
            boxList.Add(boxInfo);
        }

        public void UpdateBoxTracker()
        {
            boxList.Clear();
            foreach (UIElement element in InfiniteCanvas.Children)
            {
                if (element is DraggableBox box)
                {
                    string boxInfo = $"Box {boxList.Count + 1}: ({Canvas.GetLeft(box):F0}, {Canvas.GetTop(box):F0})";
                    boxList.Add(boxInfo);
                }
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
                        // First box selection
                        firstSelectedBox = selectedBox;
                        SelectedBoxDetails.Text = "Select the second box to connect";
                    }
                    else if (firstSelectedBox != selectedBox)
                    {
                        // Second box selection - create the connection
                        CreateConnectionBetweenBoxes(firstSelectedBox, selectedBox);

                        // Reset connection creation mode
                        isAddingConnection = false;
                        firstSelectedBox = null;
                        SelectedBoxDetails.Text = "Connection created";
                        Mouse.OverrideCursor = null;

                        // Re-enable the Add Connection button
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
                    // Normal box selection behavior
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

            // Display properties of connected boxes
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

                // Update all connected lines
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

                // Change cursor to indicate selection mode
                Mouse.OverrideCursor = Cursors.Hand;

                // Disable the Remove Connection button while in selection mode
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
            // Remove the connection from both boxes
            box.RemoveConnection(connectionToRemove.ConnectedBox);
            connectionToRemove.ConnectedBox.RemoveConnection(box);

            // Remove the line from the canvas
            InfiniteCanvas.Children.Remove(connectionToRemove.ConnectionLine);

            // Close the popup
            connectionSelectionPopup.IsOpen = false;

            // Update UI
            SelectedBoxDetails.Text = "Connection removed";
            UpdateSelectedBoxDetails(box);

            ResetRemoveConnectionState();
        }

        private void ResetRemoveConnectionState()
        {
            isRemovingConnection = false;
            selectedBoxForRemoval = null;
            Mouse.OverrideCursor = null;

            // Re-enable the Remove Connection button
            var removeConnectionButton = FindName("RemoveConnectionButton") as Button;
            if (removeConnectionButton != null)
            {
                removeConnectionButton.IsEnabled = true;
            }
        }
    }
}