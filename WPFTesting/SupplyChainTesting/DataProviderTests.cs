using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SupplyChainTesting.MockClasses;
using WPFTesting.Data;
using WPFTesting.Models;
using WPFTesting.Shapes;
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
        s.Products = new ObservableCollection<Product>() {
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
        Assert.That(model.EndpointList.Count.Equals(1));
        Assert.That(model.SupplierList.Count.Equals(3));
    }
    [Test]
    public void EndpointRecivesShippmentsToComponentInventory()
    {
        SupplyChainViewModel model = setupTest();
        model.ShipmentList.Remove(model.ShipmentList.First());
        model.ShipmentList.First().Receiver = model.EndpointList.First().supplier;
        IVendor supplier = model.ShipmentList[0].Sender;
        EndpointNode endPoint = (EndpointNode)model.ShipmentList[0].Receiver;
        model.AdvanceTime();
        Product endProd = endPoint.ComponentInventory.FirstOrDefault(p => p.ProductName == "Drill Bit") ?? new Product();
        Assert.That(endProd.ProductName.Equals("Drill Bit"));
        Assert.That(endProd.Quantity.Equals(10));
    }
    [Test]
    public void EnpointFromSaveShouldIncludeProductionLine()
    {
        SupplyChainViewModel model = setupTest();
        EndpointNode endPoint = (EndpointNode)model.EndpointList.First().supplier;
        Assert.That(endPoint.ProductionList.Count.Equals(1));
    }
    [Test]
    public void LinesArePutIntoViewModel()
    {
        SupplyChainViewModel model = setupTest();
        Shipment shipment = model.ShipmentList.First();
        Assert.That(shipment.Sender.Name.Length >= 1);
        Assert.That(shipment.Receiver.Name.Length >= 1);
        Assert.That(shipment.Products.Count.Equals(2));
	}
}
