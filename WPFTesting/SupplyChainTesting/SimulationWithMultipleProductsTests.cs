using FactorSADEfficiencyOptimizer.Models;
using FactorSADEfficiencyOptimizer.ViewModel;
using FactorySADEfficiencyOptimizer.Data;
using FactorySADEfficiencyOptimizer.Models;
using FactorySADEfficiencyOptimizer.ViewModel;
using SupplyChainTesting.MockClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SupplyChainTesting;

public class SimulationWithMultipleProductsTests
{
	[Test]
	public void TwoProductionTargetsCanCompleteWithShipmentsOrdered()
	{
		IInitializedDataProvider data = new DataProvider_FAKE_Version4();
		SupplyChainViewModel model = new SupplyChainViewModel(data);
		model.Load();

		AnalizorModel simulation = new AnalizorModel(model);
		simulation.ProductionTargets.Clear();
		simulation.ProductionTargets.Add(SimulatorTestsHelpers.MakeProductionTargetBox2(dateDue: 5, targetQuantity: 10));//makes 2 boxes per day
		simulation.ProductionTargets.Add(SimulatorTestsHelpers.MakeProductionTargetDoor(dateDue: 5, targetQuantity:  5));//makes 1 door per day
		simulation.DaysToRun = 5;
		simulation.PassTimeUntilDuration(5);

		Product Door = simulation.EndpointList.First().supplier.ProductInventory.First(p => p.ProductName == "Door"); 
		Product Box = simulation.EndpointList.First().supplier.ProductInventory.First(p => p.ProductName == "Box");
		Assert.That(Box.Quantity, Is.EqualTo(10));
		//Assert.That(Door.Quantity, Is.EqualTo(5)); // Bug, When it has two products to make, it will make the single on just fine, but when there is one that 
												   // makes 2 a day, it will start to double how many it makes in a day (starting on day 2, it isn't doubling on the first day)
												   // noticable at line 128 of Endpoint node "_productInventory.Where(x => x.ProductName == pl.ResultingProduct.ProductName).First().Quantity += pl.ResultingProduct.Quantity;"
												   // 
		//TODO: Fix the apgorithm to work with multiple products made in a day....
	}
	[Test]
	public void ProdutionTargetsCanShareComponentsAndStillOrderEnough()
	{
		IInitializedDataProvider data = new DataProvider_FAKE_Version4();
		SupplyChainViewModel model = new SupplyChainViewModel(data);
		model.Load();

		var end = model.EndpointList.First();
		((EndpointNode)end.supplier).ComponentInventory.Add(new Product()
		{
			Quantity = 300,
			ProductName = "Glue"
		});
		var prodList = ((EndpointNode)end.supplier).ProductionList;
		Assert.That(prodList.Count, Is.EqualTo(2));
		var CompList1 = prodList.First().Components;
		prodList.Last().ResultingProduct.Quantity = 1;
		var CompList2 = prodList.Last().Components;
		// both require 10 Wood. first requires 12 Screws, last requires 10 Glue
		// end starts with only 10 Wood
		Assert.That(CompList1.Count, Is.EqualTo(2));
		Assert.That(CompList2.Count, Is.EqualTo(2));
		model.ShipmentList.Clear();
		AnalizorModel simulation = new AnalizorModel(model);
		( simulation.ProductionTargets.First().ProductTarget ?? new Product() ).Quantity = 6;
		simulation.ProductionTargets.First().TargetQuantity = 6;
		( simulation.ProductionTargets.Last().ProductTarget ?? new Product() ).Quantity = 6;
		simulation.ProductionTargets.Last().TargetQuantity = 6;
		simulation.DaysToRun = 6;
		simulation.PassTimeUntilDuration(6);

		Assert.That(simulation.ShipmentList.Count, Is.EqualTo(1));

		var product = simulation.ShipmentList.First().Products.First();

		Assert.That(product.ProductName, Is.EqualTo("Wood"));
		Assert.That(product.Quantity, Is.EqualTo(110));

		Assert.That(simulation.ProductionTargets.First().Status, Is.EqualTo(StatusEnum.Success));
		Assert.That(simulation.ProductionTargets.Last().Status, Is.EqualTo(StatusEnum.Success));

	}
}
