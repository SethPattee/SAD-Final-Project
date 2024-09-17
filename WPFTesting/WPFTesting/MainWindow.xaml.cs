using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;

namespace YourNamespace
{
    public partial class MainWindow : Window
    {
        private ObservableCollection<string> boxList = new ObservableCollection<string>();
        public event EventHandler BoxChanged;
        private bool IsFileChecked = false;
        private bool IsEditChecked = false;

        public MainWindow()
        {
            InitializeComponent();
            AddDraggableBoxes();
            BoxList.ItemsSource = boxList;

        }

        private void AddDraggableBoxes()
        {
            // Create instances of the draggable boxes
            DraggableBox box1 = new DraggableBox();
            DraggableBox box2 = new DraggableBox();

            // Add the boxes to the canvas
            InfiniteCanvas.Children.Add(box1);
            InfiniteCanvas.Children.Add(box2);

            // Set initial positions for the boxes
            Canvas.SetLeft(box1, 100);
            Canvas.SetTop(box1, 100);
            Canvas.SetLeft(box2, 300);
            Canvas.SetTop(box2, 300);

            // Create a line to connect the two boxes
            Line line = CreateLineBetweenBoxes(box1, box2);

            // Add the line to the canvas
            InfiniteCanvas.Children.Add(line);

            // Add boxes to the tracker
            AddBoxToTracker(box1);
            AddBoxToTracker(box2);

            // Attach event handlers for box selection
            box1.MouseDown += Box_MouseDown;
            box2.MouseDown += Box_MouseDown;

            box1.BoxChanged += Box_Changed;
            box2.BoxChanged += Box_Changed;
        }


        private Line CreateLineBetweenBoxes(DraggableBox box1, DraggableBox box2)
        {
            // Create the line and set its initial coordinates
            Line line = new Line
            {
                //Generated to find the center of the box for the line.
                //X1 = Canvas.GetLeft(box1) + box1.Width / 2,
                //Y1 = Canvas.GetTop(box1) + box1.Height / 2,
                //X2 = Canvas.GetLeft(box2) + box2.Width / 2,
                //Y2 = Canvas.GetTop(box2) + box2.Height / 2,
                X1 = Canvas.GetLeft(box1),
                Y1 = Canvas.GetTop(box1) ,
                X2 = Canvas.GetLeft(box2),
                Y2 = Canvas.GetTop(box2) ,

                Stroke = System.Windows.Media.Brushes.Black,
                StrokeThickness = 2
            };

            // Create a connection object to track the boxes connected by this line
            var connection = new LineConnection
            {
                StartBox = box1,
                EndBox = box2
            };

            // Store the connection in the line's Tag property
            line.Tag = connection;

            // Add the line to both boxes' ConnectedLines lists
            box1.ConnectedLines.Add(line);
            box2.ConnectedLines.Add(line);

            return line;
        }
        // Event handler for the "Add Box" button click
        private void AddBox_Click(object sender, RoutedEventArgs e)
        {
            AddNewBox();
        }

        //private void ToolRibbon_PreviewMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        //{
        //    if(sender is Button FileButton) {
        //        IsFileChecked = true;
        //        IsEditChecked = false;
        //    }
        //    if(sender is Button EditButton)
        //    {
        //        IsFileChecked = false;
        //        IsEditChecked = true;
        //    }
        //    else
        //    {
        //        IsFileChecked = false;
        //        IsEditChecked = false;
        //    }
        //}

        // Method to add a new DraggableBox to the canvas
        private void AddNewBox()
        {
            DraggableBox newBox = new DraggableBox();
            Canvas.SetLeft(newBox, 100);
            Canvas.SetTop(newBox, 100);
            newBox.Width = 100;
            newBox.Height = 50;
            InfiniteCanvas.Children.Add(newBox);

            // Add the new box to the tracker
            AddBoxToTracker(newBox);

            // Attach event handler for box selection
            newBox.MouseDown += Box_MouseDown;
            newBox.BoxChanged += Box_Changed;
        }
        private void AddBoxToTracker(DraggableBox box)
        {
            string boxInfo = $"Box {boxList.Count + 1}: ({Canvas.GetLeft(box)}, {Canvas.GetTop(box)})";
            boxList.Add(boxInfo);
        }

        private void UpdateBoxTracker()
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
                UpdateSelectedBoxDetails(selectedBox);
            }
        }

        private void UpdateSelectedBoxDetails(DraggableBox box)
        {
            SelectedBoxDetails.Text = $"Position: ({Canvas.GetLeft(box):F0}, {Canvas.GetTop(box):F0})\n" +
                                      $"Size: {box.Width:F0}x{box.Height:F0}\n" +
                                      $"Color: {(box.boxBorder.Background as System.Windows.Media.SolidColorBrush)?.Color}";
        }

        private void Box_Changed(object sender, EventArgs e)
        {
            UpdateBoxTracker();
            if (sender is DraggableBox changedBox)
            {
                UpdateSelectedBoxDetails(changedBox);
            }
        }
    }
}
