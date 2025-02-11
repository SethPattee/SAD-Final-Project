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
		simulation.ShipmentList.First().Products = new () { };

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
	public void AnalyzerGetsDailyQuotaForTargetProduct()
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
	[Test]
	public void AnalyzerGetsNeededComponentsForTargetProduct()
	{
		var model = setupTest();
		AnalizorModel simulation = new AnalizorModel(model);
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
		EndpointUIValues endpoint = simulation.EndpointList.First();
		ProductLine productLine = ((EndpointNode)endpoint.supplier).ProductionList.First();
		List<Product> neededProducts =  simulation.GetNeededComponentQuantitiesForTarget( newtarg, productLine);
		Assert.That(neededProducts.Count, Is.EqualTo(2));
		Product wood = neededProducts.FirstOrDefault(p => p.ProductName == "wood") ?? new Product();
		Product screws = neededProducts.FirstOrDefault(p => p.ProductName == "screws") ?? new Product();
		Assert.That(wood.Quantity, Is.EqualTo(10));
		Assert.That(screws.Quantity, Is.EqualTo(12));
	}
	[Test]
	public void AnalyzerOrdersNeededComponentsForTargetProduct()
	{
		var model = setupTest();
		AnalizorModel simulation = new AnalizorModel(model);
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
		endpoint.supplier.ComponentInventory.First(c => c.ProductName == "screws").Quantity = 0;
		simulation.OrderMissingComponents();
		ObservableCollection<Shipment> shipments = simulation.ShipmentList;
		Assert.That(shipments.Count, Is.EqualTo(2));
		Shipment ship = shipments.First(s => s.Sender.Name == "Vendor 3");
		Assert.That(ship.Products.Count, Is.EqualTo(1));
		Assert.That(ship.Products.First().ProductName, Is.EqualTo("screws"));
		Assert.That(ship.Products.First().Quantity, Is.EqualTo(12));
	}
	[Test]
	public void AnalyzersPlaceOrderForMethodPlacesAnOrder()
	{
		var model = setupTest();
		AnalizorModel simulation = new AnalizorModel(model);
		Product product = new Product()
		{
			Quantity = 10,
			ProductName = "Drill Bit"
		};
		EndpointUIValues endpoint = simulation.EndpointList.First();
		simulation.PlaceOrderFor(product,endpoint);
		//Assert.That(simulation.ShipmentList.Count, Is.EqualTo(2));
		Shipment shipment = simulation.ShipmentList.FirstOrDefault(s => s.Products.FirstOrDefault(p => p.ProductName == "Drill Bit") != null) ;
		Assert.That(shipment.Receiver.Name, Is.EqualTo(endpoint.supplier.Name));
		Assert.That(shipment.Products.Count, Is.EqualTo(1));
		Assert.That(shipment.Products.First().ProductName, Is.EqualTo("Drill Bit"));
	}
	[Test]
	public void AnalyzerWillProduceEnoughForOneProductionTarget()
	{
        var model = setupTest();
        AnalizorModel simulation = new AnalizorModel(model);
        ProductionTarget newtarg = new ProductionTarget()
        {
            DueDate = 10,
            InitAmount = 0,
            IsTargetEnabled = true,
            Status = 0,
            ProductTarget = new Product()
            {
                Quantity = 0,
                ProductName = "box"
            },
            TargetQuantity = 10
        }; //box takes 12 screws and 10 wood
           // endpoint has 20 screws and 1000 wood
           // 'Vendor 3'  has screws 
        (simulation.SupplierList
			.FirstOrDefault(s => s.supplier.Name == "Vendor 3")
			?.supplier.ProductInventory
			.FirstOrDefault(p => p.ProductName == "screws")
			?? new Product())
			.Quantity += 120;  //  give it more for the test

		// there is one shipment bringing 10 wood and 10 nails from 'Vendor 3' to 'Vendor 2'
		// TODO: We EXPECT to have added a shipment for more screws from 'Vendor 3' to 'Endpoint Name'
		simulation.PassTimeUntilDuration(10);
		Assert.That(simulation.EndpointList.FirstOrDefault()?.supplier.ProductInventory.FirstOrDefault(p => p.ProductName == "box")?.Quantity, Is.EqualTo(10));




    }
}
