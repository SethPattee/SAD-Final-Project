using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Linq;

public class InfiniteCanvas : Panel
{
    public List<ResizableBox> Boxes { get; private set; } = new List<ResizableBox>();
    private List<Connection> connections = new List<Connection>();
    private ResizableBox selectedBox1 = null;
    private ResizableBox selectedBox2 = null;

    public event EventHandler BoxAdded;
    public event EventHandler<ResizableBox> BoxSelected;

    public InfiniteCanvas()
    {
        this.DoubleBuffered = true;
        this.AutoScroll = true;
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);

        foreach (var connection in connections)
        {
            e.Graphics.DrawLine(Pens.Black, connection.Start, connection.End);
        }
    }

    public void AddBox(Point location)
    {
        var box = new ResizableBox($"Box {Boxes.Count + 1}");
        box.Location = location;
        box.Selected += Box_Selected;
        box.LocationChanged += Box_LocationChanged;
        box.DeleteRequested += Box_DeleteRequested;
        Boxes.Add(box);
        this.Controls.Add(box);
        BoxAdded?.Invoke(this, EventArgs.Empty);
    }

    private void Box_Selected(object sender, EventArgs e)
    {
        var selectedBox = sender as ResizableBox;

        if (selectedBox1 == null)
        {
            selectedBox1 = selectedBox;
            selectedBox1.SetSelected(true);
        }
        else if (selectedBox2 == null && selectedBox != selectedBox1)
        {
            selectedBox2 = selectedBox;
            selectedBox2.SetSelected(true);
            CreateConnection();
        }
        else
        {
            selectedBox1.SetSelected(false);
            if (selectedBox2 != null)
                selectedBox2.SetSelected(false);

            selectedBox1 = selectedBox;
            selectedBox1.SetSelected(true);
            selectedBox2 = null;
        }

        BoxSelected?.Invoke(this, selectedBox);
    }

    private void CreateConnection()
    {
        if (selectedBox1 != null && selectedBox2 != null)
        {
            var newConnection = new Connection
            {
                Start = new Point(selectedBox1.Left + selectedBox1.Width / 2, selectedBox1.Top + selectedBox1.Height / 2),
                End = new Point(selectedBox2.Left + selectedBox2.Width / 2, selectedBox2.Top + selectedBox2.Height / 2),
                StartBox = selectedBox1,
                EndBox = selectedBox2
            };

            connections.Add(newConnection);
            Invalidate();

            // Reset selections
            selectedBox1.SetSelected(false);
            selectedBox2.SetSelected(false);
            selectedBox1 = null;
            selectedBox2 = null;
        }
    }

    private void Box_LocationChanged(object sender, EventArgs e)
    {
        var box = sender as ResizableBox;
        var affectedConnections = connections.Where(c => c.StartBox == box || c.EndBox == box).ToList();
        foreach (var connection in affectedConnections)
        {
            connection.UpdatePositions();
        }
        Invalidate();
    }

    private void Box_DeleteRequested(object sender, EventArgs e)
    {
        var box = sender as ResizableBox;
        Boxes.Remove(box);
        connections.RemoveAll(c => c.StartBox == box || c.EndBox == box);
        this.Controls.Remove(box);
        Invalidate();
    }
}

public class ResizableBox : UserControl
{
    private bool isResizing = false;
    private bool isMoving = false;
    private Point lastMousePos;
    private const int ResizeHandleSize = 10;
    public string BoxName { get; private set; }
    private bool isSelected = false;
    private ContextMenuStrip contextMenu;

    public event EventHandler Selected;
    public event EventHandler DeleteRequested;

    public ResizableBox(string name)
    {
        BoxName = name;
        this.Size = new Size(100, 100);
        this.BackColor = Color.LightBlue;
        this.Cursor = Cursors.SizeAll;

        InitializeContextMenu();
    }

    private void InitializeContextMenu()
    {
        contextMenu = new ContextMenuStrip();
        contextMenu.Items.Add("Rename", null, RenameBox);
        contextMenu.Items.Add("Change Color", null, ChangeColor);
        contextMenu.Items.Add("Delete", null, DeleteBox);

        this.ContextMenuStrip = contextMenu;
    }

    private void RenameBox(object sender, EventArgs e)
    {
        string newName = Microsoft.VisualBasic.Interaction.InputBox("Enter new name for the box:", "Rename Box", BoxName);
        if (!string.IsNullOrWhiteSpace(newName))
        {
            BoxName = newName;
            this.Invalidate();
        }
    }

    private void ChangeColor(object sender, EventArgs e)
    {
        ColorDialog colorDialog = new ColorDialog();
        if (colorDialog.ShowDialog() == DialogResult.OK)
        {
            this.BackColor = colorDialog.Color;
        }
    }

    private void DeleteBox(object sender, EventArgs e)
    {
        DeleteRequested?.Invoke(this, EventArgs.Empty);
    }

    protected override void OnMouseDown(MouseEventArgs e)
    {
        base.OnMouseDown(e);
        if (e.Button == MouseButtons.Left)
        {
            Selected?.Invoke(this, EventArgs.Empty);
            lastMousePos = e.Location;
            if (IsInResizeZone(e.Location))
            {
                isResizing = true;
                this.Cursor = Cursors.SizeNWSE;
            }
            else
            {
                isMoving = true;
            }
        }
    }

    protected override void OnMouseMove(MouseEventArgs e)
    {
        base.OnMouseMove(e);
        if (isResizing)
        {
            this.Size = new Size(
                Math.Max(ResizeHandleSize, this.Width + (e.X - lastMousePos.X)),
                Math.Max(ResizeHandleSize, this.Height + (e.Y - lastMousePos.Y))
            );
            lastMousePos = e.Location;
        }
        else if (isMoving)
        {
            this.Left += e.X - lastMousePos.X;
            this.Top += e.Y - lastMousePos.Y;
        }
        else if (IsInResizeZone(e.Location))
        {
            this.Cursor = Cursors.SizeNWSE;
        }
        else
        {
            this.Cursor = Cursors.SizeAll;
        }
    }

    protected override void OnMouseUp(MouseEventArgs e)
    {
        base.OnMouseUp(e);
        isResizing = false;
        isMoving = false;
        this.Cursor = Cursors.SizeAll;
    }

    private bool IsInResizeZone(Point p)
    {
        return p.X >= this.Width - ResizeHandleSize && p.Y >= this.Height - ResizeHandleSize;
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);
        e.Graphics.DrawString(BoxName, this.Font, Brushes.Black, new PointF(5, 5));
        e.Graphics.FillRectangle(Brushes.Gray,
            this.Width - ResizeHandleSize, this.Height - ResizeHandleSize,
            ResizeHandleSize, ResizeHandleSize);

        if (isSelected)
        {
            e.Graphics.DrawRectangle(new Pen(Color.Red, 2), 0, 0, this.Width - 1, this.Height - 1);
        }
    }

    public void SetSelected(bool selected)
    {
        isSelected = selected;
        this.Invalidate();
    }
}

public class Connection
{
    public Point Start { get; set; }
    public Point End { get; set; }
    public ResizableBox StartBox { get; set; }
    public ResizableBox EndBox { get; set; }

    public void UpdatePositions()
    {
        Start = new Point(StartBox.Left + StartBox.Width / 2, StartBox.Top + StartBox.Height / 2);
        End = new Point(EndBox.Left + EndBox.Width / 2, EndBox.Top + EndBox.Height / 2);
    }
}

public class MainForm : Form
{
    private InfiniteCanvas canvas;
    private Panel sidebar;
    private ListBox boxList;

    public MainForm()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        this.Size = new Size(800, 600);

        canvas = new InfiniteCanvas();
        canvas.Dock = DockStyle.Fill;
        canvas.BoxAdded += Canvas_BoxAdded;
        canvas.BoxSelected += Canvas_BoxSelected;

        sidebar = new Panel();
        sidebar.Dock = DockStyle.Right;
        sidebar.Width = 200;

        var newBoxButton = new Button();
        newBoxButton.Text = "New Box";
        newBoxButton.Dock = DockStyle.Top;
        newBoxButton.Click += NewBoxButton_Click;

        boxList = new ListBox();
        boxList.Dock = DockStyle.Fill;

        sidebar.Controls.Add(newBoxButton);
        sidebar.Controls.Add(boxList);

        this.Controls.Add(canvas);
        this.Controls.Add(sidebar);
    }

    private void NewBoxButton_Click(object sender, EventArgs e)
    {
        canvas.AddBox(new Point(10, 10));
    }

    private void Canvas_BoxAdded(object sender, EventArgs e)
    {
        boxList.Items.Add(canvas.Boxes.Last().BoxName);
    }

    private void Canvas_BoxSelected(object sender, ResizableBox e)
    {
        boxList.SelectedItem = e.BoxName;
    }
}