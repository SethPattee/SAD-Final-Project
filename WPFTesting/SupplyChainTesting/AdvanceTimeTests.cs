using SupplyChainTesting.MockClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPFTesting.Data;
using WPFTesting.Models;
using WPFTesting.ViewModel;

namespace SupplyChainTesting;

internal class AdvanceTimeTests
{
    [Test]
    public void AdvanceTime_with_one_shippment()
    {
        IInitializedDataProvider data = new DataProvider_FAKE();
        SupplyChainViewModel model = new SupplyChainViewModel(data);
        model.Load();
        //Model has data properly
        Assert.AreEqual(3, model.SupplierList.Count);
        Assert.AreEqual(20, model.SupplierList[0].supplier.ProductInventory[0].Quantity);
        Assert.AreEqual(20, model.SupplierList[1].supplier.ProductInventory[0].Quantity);

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

        model.AdvanceTime();
        // supplier loses 10 
        Assert.AreEqual(10, model.SupplierList[0].supplier.ProductInventory[0].Quantity);
        // recever gains 10
        Assert.AreEqual(30, model.SupplierList[1].supplier.ProductInventory[0].Quantity);
    }
}
