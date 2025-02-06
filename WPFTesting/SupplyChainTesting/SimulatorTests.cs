using FactorSADEfficiencyOptimizer.ViewModel;
using FactorSADEfficiencyOptimizer.Models;
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
	//[Test]
	//public void GivenTwoDaySimulaitonItRunsForTwoAdvanceTimes()
	//{
	//      var model = setupTest();
	//      AnalizorModel simulation = new AnalizorModel(model);
	//      ProductionTarget newtarg = new ProductionTarget()
	//      {
	//          DueDate = 2,
	//          InitAmount = 0,
	//          IsTargetEnabled = true,
	//          Status = 0,
	//          ProductTarget = new Product()
	//          {
	//              Quantity = 0,
	//              ProductName = "box"
	//          },
	//          TargetQuantity = 1
	//      };
	//      simulation.ProductionTargets.Add(newtarg);

	//simulation.PassTimeUntilDuration(2);

	//Assert.That(simulation.ProductionTargets.First().InitAmount, Is.EqualTo(1));
	//   }
	[Test]
	public void AnalyzerSetsDailyQuotaForTargetProduct()
	{
		var model = setupTest();
		AnalizorModel simulation = new AnalizorModel(model);
		Assert.That(simulation.CurrentDay, Is.EqualTo(1));
		ProductionTarget newtarg = new ProductionTarget()
		{
			DueDate = 2,
			InitAmount = 0,
			IsTargetEnabled = true,
			Status = 0,
			ProductTarget = new Product()
			{
				Quantity = 0,
				ProductName = "box"
			},
			TargetQuantity = 1
		};
		simulation.ProductionTargets.Add(newtarg);
		EndpointUIValues endpoint = simulation.EndpointList.First();
		Product product = endpoint.supplier.ProductInventory.FirstOrDefault(p => p.ProductName == "box") ?? new Product();
		int num = simulation.GetProductsNeededPerDay(newtarg, product);
		Assert.That(num, Is.EqualTo(1));
	}
}
