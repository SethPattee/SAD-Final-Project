using FactorSADEfficiencyOptimizer.Models;
using FactorySADEfficiencyOptimizer.Models;
using FactorySADEfficiencyOptimizer.Shapes;
using System.Collections.ObjectModel;

namespace FactorSADEfficiencyOptimizer.ViewModel;

public static class CopyMaker
{

    public static ObservableCollection<Product> makeShallowCopyOfProductColection(ObservableCollection<Product> products)
{
	var prods = new ObservableCollection<Product>();
	foreach (var ep in products)
	{
		Product p = ep.ShallowCopy();
		prods.Add(p);
	}
	return prods;
}

	public static Product makeShallowCopyProduct(Product ep)
	{
		Product p = new Product();
		p.ProductName = ep.ProductName;
		p.Price = ep.Price;
		p.Quantity = ep.Quantity;
		//p.Units = ep.Units;
		return p;
	}

	public static SupplierUIValues makeShallowCopySupplier(SupplierUIValues supplier)
	{
		SupplierUIValues sup = new SupplierUIValues();
		sup.supplier = new Supplier();
		sup.supplier.Name = supplier.supplier.Name;
		sup.supplier.ComponentInventory = makeShallowCopyOfProductColection(supplier.supplier.ComponentInventory);
		sup.supplier.ProductInventory = makeShallowCopyOfProductColection(supplier.supplier.ProductInventory);
		sup.supplier.Id = supplier.supplier.Id;
		sup.Position = new System.Drawing.Point();
		return sup;
	}
    public static ObservableCollection<ProductLine> makeShalowCopyColectionOfProductionLine(ObservableCollection<ProductLine> Lines)
    {
        var newLines = new ObservableCollection<ProductLine>();
        foreach (var line in Lines)
        {
            var nl = new ProductLine();
            nl.ResultingProduct = line.ResultingProduct.ShallowCopy();
            nl.ProductLineId = line.ProductLineId;
            nl.IsEnabled = line.IsEnabled;
            nl.Components = makeShallowCopyOfProductColection(line.Components);
            newLines.Add(nl);
        }
        return newLines;
    }
}