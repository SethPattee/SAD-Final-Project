using FactorSADEfficiencyOptimizer.ViewModel;
using SupplyChainTesting.MockClasses;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPFTesting.Data;
using WPFTesting.Models;
using WPFTesting.Shapes;
using WPFTesting.ViewModel;

namespace SupplyChainTesting;

public class SimulatorTests
{
	SupplyChainViewModel setupTest()
	{
		IInitializedDataProvider data = new DataProvider_FAKE_Version3();
		SupplyChainViewModel model = new SupplyChainViewModel(data);
		model.Load();
		return model;
	}
	[Test]
    public void ChangesToAnalizorModelSupplierDoesNotChangeCurrentViewModel()
    {
		var model = setupTest();
		AnalizorModel simulation = new AnalizorModel(model);
		simulation.SupplierList.First().supplier.Name = "I Don't want to find ThIs:(";
		simulation.SupplierList.First().supplier.ProductInventory = new ObservableCollection<Product>() { };

		foreach (SupplierUIValues supplier in model.SupplierList)
		{
			Assert.False(supplier.supplier.Name.Contains("ThIs:("));
			Assert.That(supplier.supplier.ProductInventory.Count, Is.Not.EqualTo(simulation.SupplierList.First().supplier.ProductInventory.Count));
		}
    }
	[Test]
	public void ChangesToAnalizorModelShipmentDoesNotChangeCurrentViewModel()
	{
		var model = setupTest();
		AnalizorModel simulation = new AnalizorModel(model);
		simulation.ShipmentList.First().Sender.Name = "I Don't want to find ThIs:(";
		simulation.ShipmentList.First().Receiver.Name = "I Don't want to find ThIs:(";
		simulation.ShipmentList.First().Products = new List<Product>() { };

		foreach (Shipment shipment in model.ShipmentList)
		{
			Assert.False(shipment.Sender.Name.Contains("ThIs:("));
			Assert.False(shipment.Receiver.Name.Contains("ThIs:("));
			Assert.That(shipment.Products.Count, Is.Not.EqualTo(simulation.ShipmentList.First().Products.Count));
		}
	}
	[Test]
	public void ChangesToAnalizorModelEndpointDoesNotChangeCurrentViewModel()
	{
		var model = setupTest();
		AnalizorModel simulation = new AnalizorModel(model);
		simulation.EndpointList.First().supplier.Name = "I Don't want to find ThIs:(";
		simulation.EndpointList.First().supplier.ProductInventory = new ObservableCollection<Product>() { };

		foreach (EndpointUIValues endpoint in model.EndpointList)
		{
			Assert.False(endpoint.supplier.Name.Contains("ThIs:("));
			Assert.That(endpoint.supplier.ProductInventory.Count, Is.Not.EqualTo(simulation.SupplierList.First().supplier.ProductInventory.Count));
		}
	}
}
