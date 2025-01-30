using WPFTesting.Models;
using WPFTesting.Shapes;

namespace WPFTesting.Data;

public class ForJsonSupplier
{
    public IVendor Supplier { get; set; }
    public int XPos {  get; set; }
    public int YPos { get; set; }
    public ForJsonSupplier()
    {
        Supplier = new Supplier();
        XPos = 10;
        YPos = 10;
    }
    public ForJsonSupplier(SupplierUIValues s)
    {
        Supplier = s.supplier;
        XPos = s.Position.X;
        YPos = s.Position.Y;
    }
}
public class FromJsonSuplier
{
    private SupplierUIValues _s = new SupplierUIValues();

    public FromJsonSuplier( ForJsonSupplier s)
    {
        _s.supplier = s.Supplier;
        _s.Position = new System.Drawing.Point() { X = s.XPos, Y = s.YPos};
    }
    public SupplierUIValues Supplier { get { return _s; } }
}

