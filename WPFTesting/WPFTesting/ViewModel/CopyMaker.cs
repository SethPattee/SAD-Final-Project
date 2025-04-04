﻿using FactorSADEfficiencyOptimizer.Models;
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

    public static ObservableCollection<DeliveryLine> makeShallowCopyOfDLCollection(ObservableCollection<DeliveryLine> dl)
    {
        var newDLs = new ObservableCollection<DeliveryLine>();
        foreach(var edl in dl)
        {
            DeliveryLine newDL = new()
            {
                DeliveryItem = edl.DeliveryItem,
                IsFulfilled = edl.IsFulfilled,
                IsRecurring = edl.IsRecurring,
                TotalPrice = edl.TotalPrice
            };
            newDLs.Add(newDL);
        }

        return newDLs;
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