using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;
using WPFTesting.Data;
using WPFTesting.Shapes;
using WPFTesting.ViewModel;

namespace YourNamespace
{
    public partial class MainWindow : Window
    {
        private ObservableCollection<string> boxList = new ObservableCollection<string>();
        private BoxViewModel _viewModel;

        public event EventHandler BoxChanged;

        public MainWindow()
        {
            InitializeComponent();
            _viewModel = new BoxViewModel(new BoxDataProvider());
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
            foreach (var box in _viewModel.Boxes) {
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

            if (draggableBoxes.Count > 1)
            {
                // Create a line to connect the two boxes
                Line line = CreateLineBetweenBoxes(draggableBoxes[0], draggableBoxes[1]);

                // Add the line to the canvas
                InfiniteCanvas.Children.Add(line);
            }
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

        // Method to add a new DraggableBox to the canvas
        private void AddNewBox()
        {
            BoxValues b = new BoxValues() { xPosition = 50, yPosition = 50, Title="new Box", Items = new List<string> { "item 1"} };
            DraggableBox newBox = new DraggableBox(b);
            Canvas.SetLeft(newBox, b.xPosition);
            Canvas.SetTop(newBox, b.yPosition);
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
