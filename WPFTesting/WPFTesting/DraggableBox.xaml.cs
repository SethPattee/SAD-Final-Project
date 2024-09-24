using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using WPFTesting.Models;
using WPFTesting.Shapes;

namespace YourNamespace
{
    public partial class DraggableBox : UserControl
    {
        private bool isDragging = false;
        private Point clickPosition;
        public event EventHandler BoxChanged;

        // Store the lines this box is connected to
        public List<BoxConnection> Connections { get; private set; } = new List<BoxConnection>();
        public List<string> Items { get; private set; } = new List<string>();

        public DraggableBox(BoxValues box)
        {
            InitializeComponent();
            this.BoxTitle.Text = box.supplier.Name;
            
            if(box != null)
                foreach(Product p in box.supplier.Products)
                {
                    Items.Add($"{p.Quantity}{p.Units ?? ""} {p.ProductName}");
                }

            this.ItemsList.ItemsSource = Items;
        }

        public void Set_Title(string title)
        {
            this.BoxTitle.Text = title;
        }

        private void Box_MouseDown(object sender, MouseButtonEventArgs e)
        {
            isDragging = true;
            clickPosition = e.GetPosition(this);
            (sender as UIElement).CaptureMouse();
        }

        private void Box_MouseMove(object sender, MouseEventArgs e)
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

                    // Update the position of the connected lines
                    UpdateConnectedLines();
                    BoxChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        private void Box_MouseUp(object sender, MouseButtonEventArgs e)
        {
            isDragging = false;
            (sender as UIElement).ReleaseMouseCapture();
        }

        // Resizing logic for resizing the box
        private void ResizeThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            double newWidth = this.Width + e.HorizontalChange;
            double newHeight = this.Height + e.VerticalChange;

            // Set minimum size to prevent resizing too small
            if (newWidth >= 50) this.Width = newWidth;
            if (newHeight >= 30) this.Height = newHeight;

            // Update the position of the connected lines
            UpdateConnectedLines();
            BoxChanged?.Invoke(this, EventArgs.Empty);
        }

        // Update the positions of all connected lines
        //private void UpdateConnectedLines()
        //{
        //    foreach (var line in ConnectedLines)
        //    {
        //        if (line.Tag is LineConnection connection)
        //        {
        //            if (connection.StartBox == this)
        //            {
        //                line.X1 = Canvas.GetLeft(this);
        //                line.Y1 = Canvas.GetTop(this);
        //            }
        //            else if (connection.EndBox == this)
        //            {
        //                line.X2 = Canvas.GetLeft(this);
        //                line.Y2 = Canvas.GetTop(this);
        //            }
        //        }
        //    }
        //}

        // Event handler for the "Delete Box" menu item
        private void DeleteBox_Click(object sender, RoutedEventArgs e)
        {
            var canvas = this.Parent as Canvas;
            if (canvas == null)
            {
                return; // Exit if the parent is not a Canvas
            }

            // Remove connected lines from the canvas and update connected boxes
            foreach (var connection in Connections.ToList()) // Use ToList() to avoid modifying the collection during iteration
            {
                if (connection?.ConnectionLine == null || connection.ConnectedBox == null)
                {
                    continue;
                }

                // Remove the line from the canvas
                canvas.Children.Remove(connection.ConnectionLine);

                // Remove this box from the connected box's connections
                connection.ConnectedBox.RemoveConnection(this);
            }

            // Clear this box's connections
            Connections.Clear();

            // Remove the box itself from the canvas
            canvas.Children.Remove(this);

            // Notify the parent window to update its box tracker
            if (Window.GetWindow(this) is MainWindow mainWindow)
            {
                mainWindow.UpdateBoxTracker();
            }
        }

        // Make sure you have this method in your DraggableBox class
        //public void RemoveConnection(DraggableBox boxToRemove)
        //{
        //    var connectionToRemove = Connections.FirstOrDefault(c => c.ConnectedBox == boxToRemove);
        //    if (connectionToRemove != null)
        //    {
        //        Connections.Remove(connectionToRemove);
        //    }
        //}

        // Event handler for the "Change Color" menu item
        private void ChangeColor_Click(object sender, RoutedEventArgs e)
        {
            var random = new Random();
            byte r = (byte)random.Next(256);
            byte g = (byte)random.Next(256);
            byte b = (byte)random.Next(256);

            // Change the background color of the boxBorder to a random color
            boxBorder.Background = new SolidColorBrush(Color.FromRgb(r, g, b));
            random = new Random();
            r = (byte)random.Next(256);
            g = (byte)random.Next(256);
            b = (byte)random.Next(256);

            boxBorder.BorderBrush = new SolidColorBrush(Color.FromRgb(r, g, b));
            BoxChanged?.Invoke(this, EventArgs.Empty);

        }

        // Method to add a connection
        public void AddConnection(DraggableBox connectedBox, Line connectionLine)
        {
            var connection = new BoxConnection
            {
                ConnectedBox = connectedBox,
                ConnectionLine = connectionLine
            };
            Connections.Add(connection);
            UpdateConnectedLines();
        }

        // Method to remove a connection
        public void RemoveConnection(DraggableBox connectedBox)
        {
            var connectionToRemove = Connections.Find(c => c.ConnectedBox == connectedBox);
            if (connectionToRemove != null)
            {
                Connections.Remove(connectionToRemove);
                var canvas = this.Parent as Canvas;
                canvas?.Children.Remove(connectionToRemove.ConnectionLine);
            }
        }

        private void UpdateConnectedLines()
        {
            foreach (var connection in Connections)
            {
                var line = connection.ConnectionLine;
                var connectedBox = connection.ConnectedBox;

                Point startPoint = this.TranslatePoint(new Point(this.ActualWidth / 2, this.ActualHeight / 2), this.Parent as UIElement);
                Point endPoint = connectedBox.TranslatePoint(new Point(connectedBox.ActualWidth / 2, connectedBox.ActualHeight / 2), this.Parent as UIElement);

                line.X1 = startPoint.X;
                line.Y1 = startPoint.Y;
                line.X2 = endPoint.X;
                line.Y2 = endPoint.Y;
            }
        }

        public List<BoxProperties> GetConnectedBoxProperties()
        {
            var properties = new List<BoxProperties>();
            foreach (var connection in Connections)
            {
                properties.Add(new BoxProperties
                {
                    Title = connection.ConnectedBox.BoxTitle.Text,
                    Items = new List<string>(connection.ConnectedBox.Items),
                    ConnectedBoxes = connection.ConnectedBox.Connections.Count
                });
            }
            return properties;
        }

        // ... (keep existing methods like DeleteBox_Click and ChangeColor_Click)

        // Override OnRender to update connected lines when the box is redrawn
        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);
            UpdateConnectedLines();
        }

    }

    public class BoxConnection
    {
        public DraggableBox ConnectedBox { get; set; }
        public Line ConnectionLine { get; set; }
    }

    public class BoxProperties
    {
        public string Title { get; set; }
        public List<string> Items { get; set; }
        public int ConnectedBoxes { get; set; }
    }
}
