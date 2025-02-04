using FactorSADEfficiencyOptimizer.ViewModel;
using SupplyChainTesting.MockClasses;
using System;
using System.Collections.Generic;
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

		foreach (SupplierUIValues supplier in model.SupplierList)
		{
			Assert.False(supplier.supplier.Name.Contains("ThIs:("));
		}
    }
}
