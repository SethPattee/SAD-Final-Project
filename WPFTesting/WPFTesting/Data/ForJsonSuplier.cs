using WPFTesting.Models;
using WPFTesting.Shapes;

namespace WPFTesting.Data;

public class ForJsonSuplier
{
    public IVendor Supplier { get; set; }
    public int XPos {  get; set; }
    public int YPos { get; set; }
    public ForJsonSuplier()
    {
        Supplier = new Supplier();
        XPos = 10;
        YPos = 10;
    }
    public ForJsonSuplier(SupplierUIValues s)
    {
        Supplier = s.supplier;
        XPos = s.Position.X;
        YPos = s.Position.Y;
    }
}
public class FromJsonSuplier
{
    private SupplierUIValues _s = new SupplierUIValues();

    public FromJsonSuplier( ForJsonSuplier s)
    {
        _s.supplier = s.Supplier;
        _s.Position = new System.Drawing.Point() { X = s.XPos, Y = s.YPos};
    }
    public SupplierUIValues Supplier { get { return _s; } }
}

public class ForJsonEndpoint
{
	public IVendor Supplier { get; set; }
	public int XPos { get; set; }
	public int YPos { get; set; }
	public ForJsonEndpoint()
	{
		Supplier = new EndpointNode();
		XPos = 10;
		YPos = 10;
	}
	public ForJsonEndpoint(EndpointUIValues s)
	{
		Supplier = s.supplier;
		XPos = s.Position.X;
		YPos = s.Position.Y;
	}
}
public class FromJsonEndpoint
{
	private EndpointUIValues _e = new EndpointUIValues();

	public FromJsonEndpoint(ForJsonEndpoint e)
	{
		_e.supplier = e.Supplier;
		_e.Position = new System.Drawing.Point() { X = e.XPos, Y = e.YPos };
	}
	public EndpointUIValues Supplier { get { return _e; } }
}