using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FactorySADEfficiencyOptimizer.Models;
using FactorySADEfficiencyOptimizer.Shapes;

namespace FactorySADEfficiencyOptimizer.ViewModel;

public static class ToolsForViewModel
{
	public static void AdvanceTimeforShipmentList(ObservableCollection<Shipment> shipments)
	{
		foreach (Shipment delivery in shipments)
		{
			delivery.ProcessTime();
			// When it's time to deliver a shipment, do:
			if (delivery.TimeUntilNextDelivery <= 0)
			{
				//shipOrder
				var sentProds = delivery.Sender.ShipOrder(delivery.Products.ToList<Product>());
				List<Product> listSent = new List<Product>();
				foreach (var prod in sentProds)
				{
					listSent.Add(prod);
				}
				//Receive 
				delivery.Receiver.Receive(listSent);
				if (!delivery.IsRecurring)
					shipments.Remove(delivery);
			}
		}
	}
	public static void AdvanceTimeForEndpointList(IEnumerable<EndpointUIValues> endpoints)
	{
		foreach (var e in endpoints)
		{
			var end = (EndpointNode)e.Supplier;
			end.Process();
		}
	}
	public static void AdvancetimeForSupplierList(IEnumerable<SupplierUIValues> suppliers)
	{
		foreach (var v in suppliers)
		{
			v.Supplier.Process();
		}
	}

    public static void FillCurrentPriceInShipments(ObservableCollection<SupplierUIValues> suppliers, ObservableCollection<Shipment> Shipments)
    {
        foreach (Shipment shipment in Shipments)
        {
            foreach (Product product in shipment.Products)
            {
                product.Price = ((suppliers
                    .FirstOrDefault(s => s.Supplier.Id == shipment.Sender.Id) ?? new SupplierUIValues())
                    .Supplier.ProductInventory
                    .FirstOrDefault(p => p.ProductName == product.ProductName) ?? new Product())
                    .Price; //new products default to price 0
                product.Price = product.Price;
            }
        }
    }
}
