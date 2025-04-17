using FactorySADEfficiencyOptimizer.ViewModel;
using FactorySADEfficiencyOptimizer.Data;
using FactorySADEfficiencyOptimizer.Models;
using FactorySADEfficiencyOptimizer.Shapes;
using SupplyChainTesting.MockClasses;

namespace SupplyChainTesting;

internal class ProducedProductTests
{
	[Test]
	public void ProductionTargetCanCompleteWithProperProducedCount()
	{
		IInitializedDataProvider data = new DataProvider_FAKE_Version5();
		SupplyChainViewModel model = new SupplyChainViewModel(data);
		model.Load();

		EndpointUIValues endpoint = model.EndpointList.FirstOrDefault() ?? new EndpointUIValues();
		var endNode = (EndpointNode)endpoint.Supplier;
		DeliveryLine deliveryLine = new DeliveryLine()
		{
			DeliveryItem = new Product() { ProductName = "Door", Quantity = 2},
			TotalPrice = 550,
			IsRecurring = true,
			IsFulfilled = false,
		};

		((EndpointNode)model.EndpointList.FirstOrDefault().Supplier).ActiveDeliveryLines.Add(deliveryLine); // Should sell two doors for $550 every chance the endpoint gets

		AnalizorModel simulation = new AnalizorModel(model);
		simulation.ProductionTargets.Clear();
		simulation.ProductionTargets.Add(SimulatorTestsHelpers.MakeProductionTargetBox2(dateDue: 5, targetQuantity: 10));//makes 2 boxes per day, should stop producing on day 5
		simulation.ProductionTargets.Add(SimulatorTestsHelpers.MakeProductionTargetDoor(dateDue: 5, targetQuantity: 5));//makes 1 door per day, should stop producing on day 5

		Product wood = simulation.EndpointList.First().Supplier.ComponentInventory.First(p => p.ProductName == "Wood");
		Product glue = simulation.EndpointList.First().Supplier.ComponentInventory.First(p => p.ProductName == "Glue");
		Product screws = simulation.EndpointList.First().Supplier.ComponentInventory.First(p => p.ProductName == "Screws");
		Assert.That(wood.Quantity, Is.EqualTo(1000));
		Assert.That(screws.Quantity, Is.EqualTo(1000));
		Assert.That(glue.Quantity, Is.EqualTo(1000));

		simulation.DaysToRun = 10;
		simulation.PassTimeUntilDuration(10); // run for 10 days. Shouldn't produce anything after day 5

		wood = simulation.EndpointList.First().Supplier.ComponentInventory.First(p => p.ProductName == "Wood");
		glue = simulation.EndpointList.First().Supplier.ComponentInventory.First(p => p.ProductName == "Glue");
		screws = simulation.EndpointList.First().Supplier.ComponentInventory.First(p => p.ProductName == "Screws");
		Assert.That(wood.Quantity, Is.EqualTo(900)); // enough components to make 5 doors and 10 boxes in 5 days
		Assert.That(screws.Quantity, Is.EqualTo(940));
		Assert.That(glue.Quantity, Is.EqualTo(950));


		Product Door = simulation.EndpointList.First().Supplier.ProductInventory.First(p => p.ProductName == "Door");
		Product Box = simulation.EndpointList.First().Supplier.ProductInventory.First(p => p.ProductName == "Box");
		Assert.That(Box.Quantity, Is.EqualTo(10));
		Assert.That(Door.Quantity, Is.EqualTo(1));
		Assert.That(simulation.ProductionTargets.FirstOrDefault(pt => pt.ProductTarget?.ProductName == "Door")?.CurrentAmount, Is.EqualTo(1));
		Assert.That(simulation.ProductionTargets.FirstOrDefault(pt => pt.ProductTarget?.ProductName == "Door")?.ProducedSoFar, Is.EqualTo(5));
		//Assert.That(simulation.EndpointList.First().Profit,Is.EqualTo(1234));//profit was 1000,but isn't changing?  

	}
}
