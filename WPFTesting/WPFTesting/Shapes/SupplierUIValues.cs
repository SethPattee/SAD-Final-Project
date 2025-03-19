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
        set => _position = value;
    }

    public SupplierUIValues()
    {
        Position = new Point(50,50);
    }

    public IVendor supplier { get; set; } = new Supplier();

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged(string name)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

    public SupplierUIValues ShallowCopy()
    {
        SupplierUIValues sup = new SupplierUIValues();
        sup.supplier = new Supplier();
        sup.supplier.Name = supplier.Name;
        sup.supplier.ComponentInventory = CopyMaker.makeShallowCopyOfProductColection(supplier.ComponentInventory);
        sup.supplier.ProductInventory = CopyMaker.makeShallowCopyOfProductColection(supplier.ProductInventory);
        sup.supplier.Id = supplier.Id;
        sup.Position = new System.Drawing.Point();
        return sup;
    }
    //ToDo: add unique Id for lines to connect to
    //ToDo: add list of boxIds that we connect lines to
    //ToDo: turn into a parent child class for differnet kinds of boxes
}
