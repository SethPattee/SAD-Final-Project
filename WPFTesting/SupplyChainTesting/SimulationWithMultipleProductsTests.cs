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
		simulation.ProductionTargets.Add(SimulatorTestsHelpers.MakeProductionTargetBox2(dateDue: 5, targetQuantity: 10));//makes 2 boxes per day
		simulation.ProductionTargets.Add(SimulatorTestsHelpers.MakeProductionTargetDoor(dateDue: 5, targetQuantity:  5));//makes 1 door per day
		simulation.DaysToRun = 5;
		simulation.PassTimeUntilDuration(5);

		Product Door = simulation.EndpointList.First().supplier.ProductInventory.First(p => p.ProductName == "Door"); 
		Product Box = simulation.EndpointList.First().supplier.ProductInventory.First(p => p.ProductName == "Box");
		Assert.That(Box.Quantity, Is.EqualTo(10));
		Assert.That(Door.Quantity, Is.EqualTo(5)); // Bug, When it has two products to make, it will make the single on just fine, but when there is one that 
												   // makes 2 a day, it will start to double how many it makes in a day (starting on day 2, it isn't doubling on the first day)
												   // noticable at line 128 of Endpoint node "_productInventory.Where(x => x.ProductName == pl.ResultingProduct.ProductName).First().Quantity += pl.ResultingProduct.Quantity;"
												   // 



	}
}
