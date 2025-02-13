using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FactorySADEfficiencyOptimizer.Models;
using FactorySADEfficiencyOptimizer.Shapes;

namespace FactorSADEfficiencyOptimizer.ViewModel;

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
			var end = (EndpointNode)e.supplier;
			end.ProduceProduct();
		}
	}
	public static void AdvancetimeForSupplierList(IEnumerable<SupplierUIValues> suppliers)
	{
		foreach (var v in suppliers)
		{
			v.supplier.Process();
		}
	}
}
