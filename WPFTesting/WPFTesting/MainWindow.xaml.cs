using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;
using WPFTesting.Shapes;

namespace YourNamespace
{
    public partial class MainWindow : Window
    {
        private List<DraggableBox> boxes = new List<DraggableBox>();
        public MainWindow()
        {
            InitializeComponent();
            AddDraggableBoxes();
        }

        private void AddDraggableBoxes()
        {
            // Create instances of the draggable boxes
            DraggableBox box1 = new DraggableBox(200,150,"title 1");
            DraggableBox box2 = new DraggableBox(150,200,"title 2");
            boxes.Add(box1);
            boxes.Add(box2);
            foreach (var box in boxes) {
                InfiniteCanvas.Children.Add(box);  
                Canvas.SetLeft(box, box.Values.xPosition);
                Canvas.SetTop(box, box.Values.yPosition);
            }

            // Create a line to connect the two boxes
            Line line = CreateLineBetweenBoxes(boxes[0], boxes[1]);

            // Add the line to the canvas
            InfiniteCanvas.Children.Add(line);
        }

        private Line CreateLineBetweenBoxes(DraggableBox box1, DraggableBox box2)
        {
            // Create the line and set its initial coordinates
            Line line = new Line
            {
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
            // Create a new DraggableBox instance
            DraggableBox newBox = new DraggableBox(100,100,"new Box");

            // Set a default position for the new box (you can adjust this)
            Canvas.SetLeft(newBox, newBox.Values.xPosition); // Position the box at x = 100
            Canvas.SetTop(newBox, newBox.Values.yPosition);  // Position the box at y = 100

            // Optionally, set a size for the new box
            newBox.Width = 100;
            newBox.Height = 50;

            // Add the new box to the canvas
            InfiniteCanvas.Children.Add(newBox);
        }
    }
}
