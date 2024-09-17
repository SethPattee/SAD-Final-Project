using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using WPFTesting.Shapes;

namespace YourNamespace
{
    public partial class DraggableBox : UserControl
    {
        private bool isDragging = false;
        private Point clickPosition;
        public event EventHandler BoxChanged;

        // Store the lines this box is connected to
        public List<Line> ConnectedLines { get; private set; } = new List<Line>();
        public List<string> items = new List<string>();
        

        public DraggableBox()
        {
            InitializeComponent();
            this.BoxTitle.Text = "Box Title";
            items.Add("line 1");
            items.Add("line 2");
            this.ItemsList.ItemsSource = items;
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
        private void UpdateConnectedLines()
        {
            foreach (var line in ConnectedLines)
            {
                if (line.Tag is LineConnection connection)
                {
                    if (connection.StartBox == this)
                    {
                        line.X1 = Canvas.GetLeft(this);
                        line.Y1 = Canvas.GetTop(this);
                    }
                    else if (connection.EndBox == this)
                    {
                        line.X2 = Canvas.GetLeft(this);
                        line.Y2 = Canvas.GetTop(this);
                    }
                }
            }
        }

        // Event handler for the "Delete Box" menu item
        private void DeleteBox_Click(object sender, RoutedEventArgs e)
        {
            var canvas = this.Parent as Canvas;

            // Remove connected lines from the canvas
            foreach (var line in ConnectedLines)
            {
                canvas.Children.Remove(line);
            }

            // Remove the box itself from the canvas
            canvas.Children.Remove(this);
        }

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

    }

    // This class holds the start and end connections for a line
    public class LineConnection
    {
        public DraggableBox StartBox { get; set; }
        public DraggableBox EndBox { get; set; }
    }
}
