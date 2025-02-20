using FactorSADEfficiencyOptimizer.ViewModel;
using FactorSADEfficiencyOptimizer.Models;
using SupplyChainTesting.MockClasses;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FactorySADEfficiencyOptimizer.Data;
using FactorySADEfficiencyOptimizer.Models;
using FactorySADEfficiencyOptimizer.Shapes;
using FactorySADEfficiencyOptimizer.ViewModel;
using FactorySADEfficiencyOptimizer.Models.AnalyzerTrackers;

namespace SupplyChainTesting;

public class SimulatorTests
{
	
	[Test]
    public void ChangesToAnalizorModelSupplierDoesNotChangeCurrentViewModel()
    {
		var model = SimulatorTestsHelpers.setupTest();
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
		var model = SimulatorTestsHelpers.setupTest();
		AnalizorModel simulation = new AnalizorModel(model);
		simulation.ShipmentList.First().Sender.Name = "I Don't want to find ThIs:(";
		simulation.ShipmentList.First().Receiver.Name = "I Don't want to find ThIs:(";
		simulation.ShipmentList.First().Products = new ObservableCollection<Product>() { };

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
		var model = SimulatorTestsHelpers.setupTest();
		AnalizorModel simulation = new AnalizorModel(model);
		simulation.EndpointList.First().supplier.Name = "I Don't want to find ThIs:(";
		simulation.EndpointList.First().supplier.ProductInventory = new ObservableCollection<Product>() { };

		foreach (EndpointUIValues endpoint in model.EndpointList)
		{
			Assert.False(endpoint.supplier.Name.Contains("ThIs:("));
			Assert.That(endpoint.supplier.ProductInventory.Count, Is.Not.EqualTo(simulation.SupplierList.First().supplier.ProductInventory.Count));
		}
	}
	[Test]
	public void AdvanceTimeInSimulationDoesNotChangeTheViewModel()
	{
		var model = SimulatorTestsHelpers.setupTest();
		AnalizorModel simulation = new AnalizorModel(model);
		int runTime = 5;
		ProductionTarget newtarg = SimulatorTestsHelpers.MakeProductionTargetBox(dateDue: runTime, targetQuantity: runTime);
		simulation.ProductionTargets.Add(newtarg);
		simulation.EndpointList.First().supplier.ComponentInventory.First(c => c.ProductName == "screws").Quantity = runTime * 12; //have enough screws to make the product
		var prodInModel = model.EndpointList.First().supplier.ProductInventory.First();
		var prodInSimulation = simulation.EndpointList.First().supplier.ProductInventory.First();
		Assert.That(prodInModel.ProductName, Is.EqualTo(prodInSimulation.ProductName));
		Assert.That(prodInModel.ProductName, Is.EqualTo("box"));
		Assert.That(prodInModel.Quantity, Is.EqualTo(prodInSimulation.Quantity));
		var modelsPrevQuant = prodInModel.Quantity;
		var simPrevQuant = prodInSimulation.Quantity;
		simulation.PassTimeUntilDuration(runTime);
		prodInModel = model.EndpointList.First().supplier.ProductInventory.First();
		prodInSimulation = simulation.EndpointList.First().supplier.ProductInventory.First();
		Assert.That(prodInModel.Quantity, Is.EqualTo(modelsPrevQuant));
		Assert.That(prodInSimulation.Quantity, Is.EqualTo(simPrevQuant + runTime)); //should have made a new product per day


	}
	[Test]
	public void AnalyzerGetsDailyQuotaForTargetProduct()
	{
		var model = SimulatorTestsHelpers.setupTest();
		AnalizorModel simulation = new AnalizorModel(model);
		Assert.That(simulation.CurrentDay, Is.EqualTo(1));
		ProductionTarget newtarg = SimulatorTestsHelpers.MakeProductionTargetBox(dateDue: 2, targetQuantity: 1);
		simulation.ProductionTargets.Add(newtarg);
		EndpointUIValues endpoint = simulation.EndpointList.First();
		Product product = endpoint.supplier.ProductInventory.FirstOrDefault(p => p.ProductName == "box") ?? new Product();
		int num = simulation.GetProductsNeededPerDay(newtarg, product);
		Assert.That(num, Is.EqualTo(1));
	}
	[Test]
	public void AnalyzerGetsNeededComponentsForTargetProduct()
	{
		var model = SimulatorTestsHelpers.setupTest();
		AnalizorModel simulation = new AnalizorModel(model);
		ProductionTarget newtarg = SimulatorTestsHelpers.MakeProductionTargetBox(dateDue: 2, targetQuantity: 1);
		simulation.ProductionTargets.Add(newtarg);
		EndpointUIValues endpoint = simulation.EndpointList.First();
		ProductLine productLine = ((EndpointNode)endpoint.supplier).ProductionList.First();
		ObservableCollection<Product> neededProducts =  simulation.GetNeededComponentQuantitiesForTarget( newtarg, productLine);
		Assert.That(neededProducts.Count, Is.EqualTo(2));
		Product wood = neededProducts.FirstOrDefault(p => p.ProductName == "wood") ?? new Product();
		Product screws = neededProducts.FirstOrDefault(p => p.ProductName == "screws") ?? new Product();
		Assert.That(wood.Quantity, Is.EqualTo(10));
		Assert.That(screws.Quantity, Is.EqualTo(12));
	}
	[Test]
	public void AnalyzerOrdersNeededComponentsForTargetProduct()
	{
		var model = SimulatorTestsHelpers.setupTest();
		AnalizorModel simulation = new AnalizorModel(model);
		ProductionTarget newtarg = SimulatorTestsHelpers.MakeProductionTargetBox(dateDue: 2, targetQuantity: 1);
		simulation.ProductionTargets.Add(newtarg);
		EndpointUIValues endpoint = simulation.EndpointList.First();
		endpoint.supplier.ComponentInventory.First(c => c.ProductName == "screws").Quantity = 2;
		simulation.OrderMissingComponents();
		ObservableCollection<Shipment> shipments = simulation.ShipmentList;
		Assert.That(shipments.Count, Is.EqualTo(2));
		Shipment ship = shipments.First(s => s.Sender.Name == "Vendor 3");
		Assert.That(ship.Products.Count, Is.EqualTo(1));
		Assert.That(ship.Products.First().ProductName, Is.EqualTo("screws"));
		Assert.That(ship.Products.First().Quantity, Is.EqualTo(10));
	}
	[Test]
	public void AnalyzersPlaceOrderForMethodPlacesAnOrder()
	{
		var model = SimulatorTestsHelpers.setupTest();
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
		Assert.That(shipment.Products.FirstOrDefault().Quantity, Is.EqualTo(10));
		Assert.That(shipment.Products.First().ProductName, Is.EqualTo("Drill Bit"));
	}
	[Test]
	public void AnalyzerWillProduceEnoughForOneProductionTarget()
	{
		var model = SimulatorTestsHelpers.setupTest();
		AnalizorModel simulation = new AnalizorModel(model);
		ProductionTarget newtarg = SimulatorTestsHelpers.MakeProductionTargetBox(dateDue: 10, targetQuantity: 10); //box takes 12 screws and 10 wood
												// endpoint has 20 screws and 1000 wood
												// 'Vendor 3'  has screws 
		(simulation.SupplierList
			.FirstOrDefault(s => s.supplier.Name == "Vendor 3")
			?.supplier.ProductInventory
			.FirstOrDefault(p => p.ProductName == "screws")
			?? new Product())
			.Quantity += 120;  //  give it more for the test
		(((EndpointNode)(simulation.EndpointList.FirstOrDefault() ?? new EndpointUIValues())
			.supplier ?? new EndpointNode())
			.ProductionList.FirstOrDefault() ?? new ProductLine())
			.IsEnabled = true;
		// there is one shipment bringing 10 wood and 10 nails from 'Vendor 3' to 'Vendor 2'
		simulation.ProductionTargets.Add(newtarg);
		simulation.PassTimeUntilDuration(10);
		var boxProd = simulation.EndpointList.FirstOrDefault()?.supplier.ProductInventory.FirstOrDefault(p => p.ProductName == "box") ?? new Product();
		Assert.That(boxProd.Quantity, Is.EqualTo(10));// we make target product amount
		var shipment = simulation.ShipmentList.FirstOrDefault(s => s.Products.FirstOrDefault()?.ProductName == "screws") ?? new Shipment();
		var shipmentProd = shipment.Products.FirstOrDefault() ?? new Product();
		Assert.That(shipmentProd.Quantity, Is.EqualTo(100));
	}

	[Test]
	public void AnalizorModelHasAListOfIssuesThatShowsWhenErrorsWereHit()
	{
		var model = SimulatorTestsHelpers.setupTest();
		AnalizorModel simulation = new AnalizorModel(model);
		SimulatorTestsHelpers.SetUpModelForChangeLogsTenDaySim(simulation);// will find Issue, Solution is to order another shipment
		Assert.That(simulation.IssueLog.Count, Is.EqualTo(1));
		Assert.That(simulation.IssueLog.First().Solution.Action, Is.EqualTo(ActionEnum.addedShipment));
		Assert.That(simulation.ChangeLog.Count, Is.EqualTo(1));
		Assert.That(simulation.ChangeLog.First().Action, Is.EqualTo(ActionEnum.addedShipment));
		Assert.That(simulation.ChangeLog.First().shipmentReceiver, Is.EqualTo(simulation.EndpointList.First()));
		Assert.That(simulation.ChangeLog.First().neededProduct.ProductName, Is.EqualTo("screws"));
	}

	//	Things we should test with similar set up
	//	var shipment = simulation.ShipmentList.FirstOrDefault(s => s.Products.FirstOrDefault()?.ProductName == "screws") ?? new Shipment();
	//	var shipmentProd = shipment.Products.FirstOrDefault() ?? new Product();
	//	Assert.That(shipmentProd.Quantity, Is.EqualTo(28));
	//	// should have a failure status for produciton Target
	//	Assert.That(newtarg.Status, Is.EqualTo(StatusEnum.Failure));
	[Test]
	public void ModelSetsTargetToSuccessWhenSimulationCompletes()
	{
		var model = SimulatorTestsHelpers.setupTest();
		AnalizorModel simulation = new AnalizorModel(model);
		SimulatorTestsHelpers.SetUpModelForChangeLogsTenDaySim(simulation);

		Assert.That(simulation.ProductionTargets.First().Status, Is.EqualTo(StatusEnum.Success));
	}
}
