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
using Microsoft.VisualBasic;

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
	[Test]
	public void AnalyzerGetsDailyQuotaForTargetProduct()
	{
		var model = setupTest();
		AnalizorModel simulation = new AnalizorModel(model);
		Assert.That(simulation.CurrentDay, Is.EqualTo(1));
        ProductionTarget newtarg = MakeProductionTarget(targetQuantity: 1, dueDay: 2);
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
        ProductionTarget newtarg = MakeProductionTarget(targetQuantity: 1, dueDay: 2);
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
		var model = setupTest();
		AnalizorModel simulation = new AnalizorModel(model);
        ProductionTarget newtarg = MakeProductionTarget(targetQuantity: 1, dueDay: 2);
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
		Assert.That(shipment.Products.FirstOrDefault().Quantity, Is.EqualTo(10));
		Assert.That(shipment.Products.First().ProductName, Is.EqualTo("Drill Bit"));
	}
	[Test]
	public void AnalyzerWillProduceEnoughForOneProductionTarget()
    {
        var model = setupTest();
        AnalizorModel simulation = new AnalizorModel(model);
        ProductionTarget newtarg = MakeProductionTarget(targetQuantity: 10, dueDay: 10); //box takes 12 screws and 10 wood
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

    private static ProductionTarget MakeProductionTarget(int targetQuantity, int dueDay)
    {
        return new ProductionTarget()
        {
            DueDate = dueDay,
            InitAmount = 0,
            IsTargetEnabled = true,
            Status = 0,
            ProductTarget = new Product()
            {
                Quantity = 0,
                ProductName = "box"
            },
            TargetQuantity = targetQuantity,
        };
    }

    [Test]
	public void AnalizorModelHasAListOfIssuesThatShowsWhenErrorsWereHit()
	{
        var model = setupTest();
		AnalizorModel simulation = new AnalizorModel(model);
		Assert.That(simulation.ChangeLog, Is.Empty);
		Assert.That(simulation.IssueLog, Is.Empty);
        ProductionTarget newtarg = MakeProductionTarget(targetQuantity: 10, dueDay: 10);
        (simulation.SupplierList
			.FirstOrDefault(s => s.supplier.Name == "Vendor 3")
			?.supplier.ProductInventory
			.FirstOrDefault(p => p.ProductName == "screws")
			?? new Product())
			.Quantity += 200; 
		simulation.ProductionTargets.Add(newtarg);
		simulation.PassTimeUntilDuration(10); // will find Issue, Solution is to order another shipment
        Assert.That(simulation.IssueLog.Count, Is.EqualTo(1));
		Assert.That(simulation.IssueLog.First().Solution.Action, Is.EqualTo(ActionEnum.addedShipment));
		Assert.That(simulation.ChangeLog.Count, Is.EqualTo(1));
		Assert.That(simulation.ChangeLog.First().Action, Is.EqualTo(ActionEnum.addedShipment));
		Assert.That(simulation.ChangeLog.First().shipmentReceiver, Is.EqualTo(simulation.EndpointList.First()));
		Assert.That(simulation.ChangeLog.First().neededProduct.ProductName, Is.EqualTo("screws"));
    }

    //	Things we should test with similar
    //	var shipment = simulation.ShipmentList.FirstOrDefault(s => s.Products.FirstOrDefault()?.ProductName == "screws") ?? new Shipment();
    //	var shipmentProd = shipment.Products.FirstOrDefault() ?? new Product();
    //	Assert.That(shipmentProd.Quantity, Is.EqualTo(28));
    //	// should have a failure status for produciton Target
    //	Assert.That(newtarg.Status, Is.EqualTo(StatusEnum.Failure));

}
