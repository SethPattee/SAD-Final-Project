using FactorSADEfficiencyOptimizer.Models;
using FactorSADEfficiencyOptimizer.ViewModel;
using FactorySADEfficiencyOptimizer.Data;
using FactorySADEfficiencyOptimizer.Models;
using FactorySADEfficiencyOptimizer.Models.AnalyzerTrackers;
using FactorySADEfficiencyOptimizer.Shapes;
using FactorySADEfficiencyOptimizer.ViewModel;
using SupplyChainTesting;
using SupplyChainTesting.MockClasses;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

internal static class SimulatorTestsHelpers
{

	public static ProductionTarget MakeProductionTargetBox(int dateDue, int targetQuantity)
	{
		return new ProductionTarget()
		{
			DueDate = dateDue,
			CurrentAmount = 0,
			IsTargetEnabled = true,
			Status = 0,
			ProductTarget = new Product()
			{
				Quantity = 0,
				ProductName = "box"
			},
			TargetQuantity = targetQuantity
		};
	}
	public static SupplyChainViewModel setupTest()
	{
		IInitializedDataProvider data = new DataProvider_FAKE_Version3();
		SupplyChainViewModel model = new SupplyChainViewModel(data);
		model.Load();
		return model;
	}

	public static void SetUpModelForChangeLogsTenDaySim(AnalizorModel simulation)
	{
		ProductionTarget newtarg = SimulatorTestsHelpers.MakeProductionTargetBox(targetQuantity: 10, dateDue: 10);
		(simulation.SupplierList
			.FirstOrDefault(s => s.supplier.Name == "Vendor 3")
			?.supplier.ProductInventory
			.FirstOrDefault(p => p.ProductName == "screws")
			?? new Product())
			.Quantity += 200;
		simulation.ProductionTargets.Add(newtarg);
		simulation.PassTimeUntilDuration(10);
	}
	public static void SetUpModelForChangeLogsFiveDayTenProductFailureSim(AnalizorModel simulation)
	{
		Assert.That(simulation.ChangeLog, Is.Empty);
		Assert.That(simulation.IssueLog, Is.Empty);
		ProductionTarget newtarg = SimulatorTestsHelpers.MakeProductionTargetBox(targetQuantity: 10, dateDue: 5);
		(simulation.SupplierList
			.FirstOrDefault(s => s.supplier.Name == "Vendor 3")
			?.supplier.ProductInventory
			.FirstOrDefault(p => p.ProductName == "screws")
			?? new Product())
			.Quantity += 200;
		simulation.ProductionTargets.Add(newtarg);
		simulation.PassTimeUntilDuration(5);
	}
}