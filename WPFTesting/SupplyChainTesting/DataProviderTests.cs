using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SupplyChainTesting.MockClasses;
using WPFTesting.Data;
using WPFTesting.Models;
using WPFTesting.ViewModel;

namespace SupplyChainTesting;

public class DataProviderTests
{
    SupplyChainViewModel setupTest()
    {
        IInitializedDataProvider data = new DataProvider_FAKE_Version3();
        SupplyChainViewModel model = new SupplyChainViewModel(data);
        model.Load();
        Shipment s = new Shipment();
        s.Sender = model.SupplierList[0].supplier;
        s.Receiver = model.SupplierList[1].supplier;
        s.Products = new List<Product>() {
            new Product()
                        {
                            Quantity=10,
                            ProductName="Drill Bit"
                        },
                        new Product()
                        {
                            Quantity=5,
                            ProductName="Saw Blade 2-pack",
                            Units="kg"
                        }
        };
        model.ShipmentList.Add(s);
        return model;
    }
    [Test]
    public void ViewmodelTakesInBothSuppliersAndEndpoints()
    {
        SupplyChainViewModel model = setupTest();
        //Assert.That(model.SupplierList.Count is 3);// just didn't tell me good feedvack
        Assert.AreEqual(1,  model.EndpointList.Count);
        Assert.AreEqual(3, model.SupplierList.Count);
    }
}
