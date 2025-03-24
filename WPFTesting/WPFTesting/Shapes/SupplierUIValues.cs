using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FactorSADEfficiencyOptimizer.ViewModel;
using FactorySADEfficiencyOptimizer.Models;

namespace FactorySADEfficiencyOptimizer.Shapes;

public class SupplierUIValues : INotifyPropertyChanged
{
    private Point _position;
    public Point Position
    {
        get => _position;
        set {
            _position = value;
            OnPropertyChanged(nameof(Position));
        }
    }

    public SupplierUIValues()
    {
        Position = new Point(50,50);
    }


    private IVendor _supplier = new Supplier();
    public IVendor Supplier {
        get { return _supplier; } 
        set
        {
            _supplier = value;
            OnPropertyChanged(nameof(Supplier));
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged(string name)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

    public SupplierUIValues ShallowCopy()
    {
        SupplierUIValues sup = new SupplierUIValues();
        sup.Supplier = new Supplier();
        sup.Supplier.Name = Supplier.Name;
        sup.Supplier.ComponentInventory = CopyMaker.makeShallowCopyOfProductColection(Supplier.ComponentInventory);
        sup.Supplier.ProductInventory = CopyMaker.makeShallowCopyOfProductColection(Supplier.ProductInventory);
        sup.Supplier.Id = Supplier.Id;
        sup.Position = new System.Drawing.Point();
        return sup;
    }
    //ToDo: add unique Id for lines to connect to
    //ToDo: add list of boxIds that we connect lines to
    //ToDo: turn into a parent child class for differnet kinds of boxes
}
