using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPFTesting.Models;

namespace WPFTesting.Shapes;

public class SupplierUIValues : INotifyPropertyChanged
{
    private int _xPosition;
    private int _yPosition;
    public int xPosition
    {
        get => _xPosition;
        set
        {
            _xPosition = value;
            OnPropertyChanged(nameof(xPosition));
        }
    }
    public int yPosition
    {
        get => _yPosition;
        set
        {
            _yPosition = value;
            OnPropertyChanged(nameof(yPosition));
        }
    }
    public IVendor supplier { get; set; } = new Supplier();

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged(string name)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
    //ToDo: add unique Id for lines to connect to
    //ToDo: add list of boxIds that we connect lines to
    //ToDo: turn into a parent child class for differnet kinds of boxes
}
